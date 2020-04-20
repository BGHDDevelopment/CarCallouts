using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CalloutAPI;
using CitizenFX.Core.Native;


namespace CarCallout
{
    
    [CalloutProperties("Pursuit of Armed Suspects", "BGHDDevelopment", "0.0.1", Probability.Low)]
    public class PursuitCallout : Callout
    {
        private Vehicle car;
        Ped driver;
        Ped passenger;

        public PursuitCallout()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Pursuit of Armed Suspects";
            CalloutDescription = "Suspects just robbed a person with weapons. They are fleeing.";
            ResponseCode = 3;
            StartDistance = 150f;
        }

        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            passenger.Weapons.Give(WeaponHash.Pistol, 20, true, false);
            driver.Weapons.Give(WeaponHash.SMG, 30, true, true);
            driver.Task.DriveTo(car, new Vector3(300f, -2005f, 20f), 1f, 35f, 524852);
            Notify("The driver and passenger are fleeing!");
            API.Wait(6000);
            car.AttachBlip();
            passenger.Task.FightAgainst(player);
        }
        public async override Task Init()
        {

            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            passenger = await SpawnPed(GetRandomPed(), Location + 1);
            car = await SpawnVehicle(VehicleHash.Adder, Location,12);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            passenger.SetIntoVehicle(car, VehicleSeat.Passenger);

            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            passenger.AlwaysKeepTask = true;
            passenger.BlockPermanentEvents = true;
        }
        private void Notify(string message)
        {
            API.BeginTextCommandThefeedPost("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandThefeedPostTicker(false, true);
        }

        public override void OnCancelBefore()
        {
            car.AttachedBlip.Delete();
        }
    }
}