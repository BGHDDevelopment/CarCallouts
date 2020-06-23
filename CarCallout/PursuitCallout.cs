using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;


namespace CarCallout
{

    [CalloutProperties("Pursuit of Armed Suspects", "BGHDDevelopment", "0.0.17")]
    public class PursuitCallout : Callout
    {
        private Vehicle car;
        Ped driver;
        Ped passenger;
        List<object> items = new List<object>();
        List<object> items2 = new List<object>();
        private string[] carList = { "speedo", "speedo2", "stanier", "stinger", "stingergt", "stratum", "stretch", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef", "minivan", "gburrito", "emperor2", "emperor"};

        public PursuitCallout()
        {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Pursuit of Armed Suspects";
            CalloutDescription = "Suspects just robbed a person with weapons. They are fleeing.";
            ResponseCode = 3;
            StartDistance = 150f;
            UpdateData();
        }
        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            passenger.Weapons.Give(WeaponHash.Pistol, 20, true, true);
            driver.Weapons.Give(WeaponHash.SMG, 30, true, true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are fleeing!");
            car.AttachBlip();
            driver.AttachBlip();
            passenger.AttachBlip();
            API.Wait(6000);
            passenger.Task.FightAgainst(player);
            dynamic data2 = await Utilities.GetPedData(passenger.NetworkId);
            dynamic data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname2 = data2.Firstname;
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~I hate cops! Let me kill you!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Do not shoot!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~To late!", 5000);
        }
        public async override Task OnAccept()
        {
            InitBlip();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            passenger = await SpawnPed(GetRandomPed(), Location + 1);
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            passenger.SetIntoVehicle(car, VehicleSeat.Passenger);
            dynamic playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            dynamic datacar = await Utilities.GetVehicleData(car.NetworkId);
            string vehicleName = datacar.VehicleName;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are driving a " + vehicleName + "!");
            
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.01;
            object SMG = new {
                Name = "SMG",
                IsIllegal = true
            };
            items.Add(SMG);
            data.items = items;
            Utilities.SetPedData(driver.NetworkId, data);
            
            //Passenger Data
            dynamic data2 = new ExpandoObject();
            data2.alcoholLevel = 0.09;
            object Pistol = new {
                Name = "Pistol",
                IsIllegal = true
            };
            items2.Add(Pistol);
            data2.items = items2;
            Utilities.SetPedData(passenger.NetworkId, data2);
            
            //Car Data
            dynamic vehicleData = new ExpandoObject();
            vehicleData.registration = false;
            Utilities.SetVehicleData(car.NetworkId,vehicleData);
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId,true);
            
            //Tasks
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
        private void DrawSubtitle(string message, int duration)
        {
            API.BeginTextCommandPrint("STRING");
            API.AddTextComponentSubstringPlayerName(message);
            API.EndTextCommandPrint(duration, false);
        }
        public override void OnCancelBefore()
        {
        }
    }
}