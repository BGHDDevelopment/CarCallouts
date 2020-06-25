using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;


namespace CarCallout
{
    
    [CalloutProperties("Stolen Police Car (Hostage) Callout", "BGHDDevelopment", "0.0.17")]
    public class StolenPoliceCarHostage : Callout
    {
        private Vehicle car;
        Ped driver, police;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};
        List<object> items = new List<object>();
        public StolenPoliceCarHostage()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Stolen Police Car (With Hostage)";
            CalloutDescription = "Someone stole a police car and took an officer hostage!";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            API.SetDriveTaskMaxCruiseSpeed(driver.GetHashCode(), 30f);
            API.SetDriveTaskDrivingStyle(driver.GetHashCode(), 524852);
            driver.Task.FleeFrom(player);
            car.AttachBlip();
            driver.AttachBlip();
            police.AttachBlip();
            police.Task.HandsUp(1000000);
            dynamic playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is fleeing!");
            dynamic data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname = data1.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Stay quiet and don't say anything!", 5000);
            dynamic data2 = await Utilities.GetPedData(police.NetworkId);
            string firstname2 = data2.Firstname;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname2 + "] ~s~Will do.... are you high?", 5000);
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Shut up!", 5000);
        }
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            police = await SpawnPed(PedHash.Hwaycop01SMY, Location + 1);
            car = await SpawnVehicle(VehicleHash.Police, Location,12);
            API.SetVehicleLights(car.GetHashCode(), 2);
            API.SetVehicleLightsMode(car.GetHashCode(), 2);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            police.SetIntoVehicle(car, VehicleSeat.Passenger);
            driver.Weapons.Give(WeaponHash.Pistol, 20, true, true);
            dynamic data = new ExpandoObject();
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            object goodItem = new {
                Name = name2,
                IsIllegal = false
            };
            object Pistol = new {
                Name = "Pistol",
                IsIllegal = true
            };
            items.Add(Pistol);
            items.Add(goodItem);
            data.items = items;
            data.drugsUsed = new bool[] {true,false,false};
            Utilities.SetPedData(driver.NetworkId,data);
            //Car Data
            dynamic vehicleData = new ExpandoObject();
            Utilities.SetVehicleData(car.NetworkId,vehicleData);
            Utilities.ExcludeVehicleFromTrafficStop(car.NetworkId,true);
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
        }
        public override void OnCancelBefore()
        {
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
    }
}