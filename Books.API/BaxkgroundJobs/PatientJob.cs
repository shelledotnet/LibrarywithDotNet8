
using Books.Domain.Data;
using Books.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.API.BaxkgroundJobs
{
    public class PatientJob : BackgroundService
    {
        private readonly ILogger<PatientJob> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public PatientJob(ILogger<PatientJob> logger,IServiceScopeFactory serviceScopeFactory)
        {
              _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while the application is not cancelled
            while (!stoppingToken.IsCancellationRequested) 
            {
                _logger.LogInformation("patient job runing ");
                await using var scope  = _serviceScopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<EmployeeManagerDbContext>();
                var jobs =await context.ImportJobs
                                            .Where(job => job.Status == JobStatus.Enqueued)
                                            .ToListAsync(stoppingToken);

                _logger.LogInformation(jobs.Count,"Total Enqueued Job ");

                foreach (var job in jobs)
                {
                    job.Status= JobStatus.Running;
                    job.StartedAt = DateTime.Now;
                    await context.SaveChangesAsync(stoppingToken);

                    try
                    {
                        using StreamReader reader = await ImportPatienceRecords(context, job, stoppingToken);
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

                await Task.Delay(10000,stoppingToken);  //10s
            }

        }

        private static async Task<StreamReader> ImportPatienceRecords(EmployeeManagerDbContext context, ImportJob? job, CancellationToken stoppingToken)
        {
            List<PatientRecord> patientRecords = new List<PatientRecord>();


            #region Please dont do this when reading a file bcos itsvery slow
            //var patients = File.ReadAllLines(Path.Combine("temp",job.FileName))
            //    .Skip(1); 
            #endregion

            #region Please do this instead
            string path = Path.Combine("temp", job.FileName);
            var reader = new StreamReader(path);
            _ = await reader.ReadLineAsync(stoppingToken);  //this skip the header implicitly

            // read the line if is not null its an object stored it in patient
            while (await reader.ReadLineAsync(stoppingToken) is { } patient)
            {
                var patientValue = patient.Split(",");
                patientRecords.Add(new PatientRecord()
                {
                    FirstName = patientValue[0],
                    LastName = patientValue[1],
                    Email = patientValue[2],
                    Address = patientValue[3],
                    Phone = patientValue[4]

                });

                //be posting to Db in modules / parallel programming ? eventually every 5 records
                //ur barch upload will depend on your use case
                if (patientRecords.Count % 5 == 0)
                {
                    await context.PatientRecords.AddRangeAsync(patientRecords);
                    await context.SaveChangesAsync();
                }
            }
            #endregion
            await context.PatientRecords.AddRangeAsync(patientRecords);
            await context.SaveChangesAsync();
            return reader;
        }
    }
}
