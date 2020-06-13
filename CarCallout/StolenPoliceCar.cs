using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;


namespace CarCallout
{
    
    [CalloutProperties("Stolen Police Car Callout", "BGHDDevelopment", "0.0.17")]
    public class StolenPoliceCar : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};
        List<object> items = new List<object>();
        public StolenPoliceCar()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitBase(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Stolen Police Car";
            CalloutDescription = "Someone stole a police car!";
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
            dynamic playerData = GetPlayerData();
            string displayName = playerData.DisplayName;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspect is fleeing!");
        }
        public async override Task Init()
        {
            OnAccept();
            driver = await SpawnPed(GetRandomPed(), Location + 2);
            car = await SpawnVehicle(VehicleHash.Police, Location,12);
            API.SetVehicleLights(car.GetHashCode(), 2);
            API.SetVehicleLightsMode(car.GetHashCode(), 2);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);
            dynamic data = new ExpandoObject();
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            object goodItem = new {
                Name = name2,
                IsIllegal = false
            };
            items.Add(goodItem);
            data.items = items;
            SetPedData(driver.NetworkId,data);
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
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