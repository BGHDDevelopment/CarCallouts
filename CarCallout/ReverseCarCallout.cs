using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;


namespace CarCallout
{
    
    [CalloutProperties("Reverse Car Callout", "BGHDDevelopment", "0.0.1", Probability.Medium)]
    public class ReverseCarCallout : Callout
    {
        private Vehicle car;
        Ped driver;

        public ReverseCarCallout()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Car Driving in Reverse";
            CalloutDescription = "A car is driving in reverse.";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            driver.Task.DriveTo(car, new Vector3(1389.76f, 3608.71f, 19.7f), 1f, 12f, 1923);
            car.AttachBlip();
        }
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.Buffalo, Location,12);
            driver.SetIntoVehicle(car, (VehicleSeat) (1));

            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
        }
        public override void OnCancelBefore()
        {
            car.AttachedBlip.Delete();
        }

    }
}