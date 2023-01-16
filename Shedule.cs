
using System;

using Quartz;


namespace test
{
    class Shedule : IJob
    {

        public Shedule()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }





        public void Execute(IJobExecutionContext context)
        {
            Form1 objn = new Form1();
            objn.synccall();
        }
    }
}
