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

    [CalloutProperties("Pursuit of Armed Suspects (Van)", "BGHDDevelopment", "0.0.17")]
    public class VanPursuit : Callout
    {
        private Vehicle car;
        Ped driver, passenger, passenger2;
        List<object> items = new List<object>();
        List<object> items2 = new List<object>();
        List<object> items3 = new List<object>();

        public VanPursuit()
        {
            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);
            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Pursuit of Armed Suspects (Van)";
            CalloutDescription = "Suspects just robbed a store with weapons. They are fleeing.";
            ResponseCode = 3;
            StartDistance = 150f;
        }
        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            driver.Weapons.Give(WeaponHash.Pistol, 20, true, true);
            passenger.Weapons.Give(WeaponHash.SMG, 150, true, true);
            passenger2.Weapons.Give(WeaponHash.SMG, 150, true, true);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 35f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are fleeing!");
            car.AttachBlip();
            driver.AttachBlip();
            passenger.AttachBlip();
            passenger2.AttachBlip();
            API.Wait(6000);
            passenger.Task.FightAgainst(player);
            passenger2.Task.FightAgainst(player);
            dynamic data2 = await GetPedData(passenger.NetworkId);
            dynamic data1 = await GetPedData(driver.NetworkId);
            string firstname2 = data2.Firstname;
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~I hate cops! Let me kill you!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~FIRE!", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~DIE!", 5000);
        }
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            passenger = await SpawnPed(GetRandomPed(), Location + 1);
            passenger2 = await SpawnPed(GetRandomPed(), Location + 1);
            car = await SpawnVehicle(VehicleHash.Speedo, Location);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            passenger2.SetIntoVehicle(car, VehicleSeat.RightRear);
            passenger.SetIntoVehicle(car, VehicleSeat.LeftRear);
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are armed and dangerous!");
            //Driver Data
            dynamic data = new ExpandoObject();
            data.alcoholLevel = 0.01;
            object Pistol = new {
                Name = "Pistol",
                IsIllegal = true
            };
            items.Add(Pistol);
            data.items = items;
            SetPedData(driver.NetworkId,data);
            
            //Passenger Data 2
            dynamic data2 = new ExpandoObject();
            data2.alcoholLevel = 0.01;
            object SMG = new {
                Name = "SMG",
                IsIllegal = true
            };
            items2.Add(SMG);
            data2.items2 = items2;
            SetPedData(passenger.NetworkId,data2);
            
            //Passenger Data
            dynamic data3 = new ExpandoObject();
            data3.alcoholLevel = 0.09;
            object SMG2 = new {
                Name = "SMG",
                IsIllegal = true
            };
            items3.Add(SMG2);
            data3.items = items3;
            SetPedData(passenger2.NetworkId,data3);
            
            //Tasks
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            passenger.AlwaysKeepTask = true;
            passenger.BlockPermanentEvents = true;
            passenger2.AlwaysKeepTask = true;
            passenger2.BlockPermanentEvents = true;
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
            { 
                if(car != null  && car.AttachedBlip != null && car.AttachedBlip.Exists())
                    car.AttachedBlip.Delete();
            }
            { 
                if(driver != null  && driver.AttachedBlip != null && driver.AttachedBlip.Exists())
                    driver.AttachedBlip.Delete();
            }
            { 
                if(passenger != null  && passenger.AttachedBlip != null && passenger.AttachedBlip.Exists())
                    passenger.AttachedBlip.Delete();
            }
            { 
                if(passenger2 != null  && passenger2.AttachedBlip != null && passenger2.AttachedBlip.Exists())
                    passenger2.AttachedBlip.Delete();
            }
        }
    }
}