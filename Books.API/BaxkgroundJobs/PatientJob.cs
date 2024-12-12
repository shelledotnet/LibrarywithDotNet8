
using Books.Domain.Data;
using Books.Domain.Entities;
using Books.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Books.API.BaxkgroundJobs
{
    public class PatientJob : BackgroundService
    {
        private readonly ILogger<PatientJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ProjectOptions _projectOptions;

        public PatientJob(ILogger<PatientJob> logger,IServiceScopeFactory serviceScopeFactory,
            IOptionsMonitor<ProjectOptions> projectOptions)
        {
              _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _projectOptions = projectOptions.CurrentValue;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_projectOptions.TurnOnJobs.Equals("ON", StringComparison.OrdinalIgnoreCase))
            {
                //while the application is not cancelled
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("patient job runing ");
                    await using var scope = _serviceScopeFactory.CreateAsyncScope();
                    var context = scope.ServiceProvider.GetRequiredService<EmployeeManagerDbContext>();
                    var jobs = await context.ImportJobs
                                                .Where(job => job.Status == JobStatus.Enqueued)
                                                .ToListAsync(stoppingToken);

                    _logger.LogInformation(jobs.Count, "Total Enqueued Job ");

                    foreach (var job in jobs)
                    {
                        job.Status = JobStatus.Running;
                        job.StartedAt = DateTime.Now;
                        await context.SaveChangesAsync(stoppingToken);

                        try
                        {
                            string path = await ImportPatienceRecords(context, job, stoppingToken);
                            if (_projectOptions.DeleteCsv.Equals("ON", StringComparison.OrdinalIgnoreCase) && File.Exists(path))
                            {
                                File.Delete(path);
                            }
                            job.Status = JobStatus.Completed;
                            job.CompletedAt = DateTime.Now;

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "PatientJob:");
                            job.Status = JobStatus.Failed;
                            job.FailedAt = DateTime.Now;
                            job.FailureReason = ex.ToString();
                        }
                        finally
                        {
                            await context.SaveChangesAsync(stoppingToken);

                        }


                    }

                    await Task.Delay(10000, stoppingToken);  //10s
                }
            }
            

        }

        private static async Task<string> ImportPatienceRecords(EmployeeManagerDbContext context, ImportJob? job, CancellationToken stoppingToken)
        {
            List<PatientRecord> patientRecords = [];


            #region Please dont do this when reading a file bcos itsvery slow
            //var patients = File.ReadAllLines(Path.Combine("temp",job.FileName))
            //    .Skip(1); 
            #endregion

            #region Please do this instead
            string path = Path.Combine("temp", job.FileName);
            using StreamReader reader = new StreamReader(path);
            _ = await reader.ReadLineAsync(stoppingToken);  //this skip the header implicitly

            // read the line if is not null its an object stored it in patient
            while (await reader.ReadLineAsync(stoppingToken) is { } patient)
            {
                stoppingToken.ThrowIfCancellationRequested(); // Respect cancellation requests
                //Added stoppingToken.ThrowIfCancellationRequested() within the loop to allow cancellation at any time.
               
                var patientValues = patient.Split(",");

                if (patientValues.Length < 5)
                {
                    // Skip invalid lines with insufficient data
                    continue;
                }
                patientRecords.Add(new PatientRecord()
                {
                    FirstName = patientValues[0],
                    LastName = patientValues[1],
                    Email = patientValues[2],
                    Address = patientValues[3],
                    Phone = patientValues[4]

                });

                //be posting to Db in modules / parallel programming ? eventually every 5 records
                //ur barch upload will depend on your use case
                if (patientRecords.Count % 5 == 0)
                {
                    //Passed stoppingToken to EF Core operations for full cancellation support.
                    await context.PatientRecords.AddRangeAsync(patientRecords,stoppingToken);
                    await context.SaveChangesAsync(stoppingToken);
                    patientRecords.Clear(); // Clear the list for the next batch
                }
            }
            #endregion

            await context.PatientRecords.AddRangeAsync(patientRecords,stoppingToken);
            await context.SaveChangesAsync(stoppingToken);
           // patientRecords.Clear(); // Not required at this moment its left over list or initial records not up to 
            return path;
        }
    }
}
