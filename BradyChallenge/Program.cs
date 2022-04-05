using BradyChallenge.InputOutputOperations;

namespace BradyChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            IPickupInputFile pickupObject = new PickupInputFile();
            pickupObject.WatchForInputFile();
        }
    }
}
