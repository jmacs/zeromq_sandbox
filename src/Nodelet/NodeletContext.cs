using System;

namespace Nodelet
{
    public class NodeletContext
    {
        public int Execute(Action execution)
        {
            var returnCode = 0;

            try
            {
                execution();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError! ");
                var inner = ex;
                while (inner != null)
                {
                    Console.WriteLine("{0}: {1}", inner.GetType(), inner.Message);
                    Console.WriteLine(inner.StackTrace);
                    inner = inner.InnerException;
                }
                
                returnCode = 1;
            }

            return returnCode;
        }
    }
}
