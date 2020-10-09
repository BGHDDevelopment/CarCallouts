using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;


namespace CarCallout
{
    
    [CalloutProperties("Slow Driver Callout", "BGHDDevelopment", "0.0.17")]
    public class SlowDriver : Callout
    {
        private Vehicle car;
        Ped driver;
        private string[] goodItemList = { "Open Soda Can", "Pack of Hotdogs", "Dog Food", "Empty Can", "Phone", "Cake", "Cup of Noodles", "Water Bottle", "Pack of Cards", "Outdated Insurance Card", "Pack of Pens", "Phone", "Tablet", "Computer", "Business Cards", "Taxi Business Card", "Textbooks", "Car Keys", "House Keys", "Keys", "Folder"};
        List<object> items2 = new List<object>();
        private string[] carList = { "speedo", "speedo2", "stanier", "stinger", "stingergt", "stratum", "stretch", "taco", "tornado", "tornado2", "tornado3", "tornado4", "tourbus", "vader", "voodoo2", "dune5", "youga", "taxi", "tailgater", "sentinel2", "sentinel", "sandking2", "sandking", "ruffian", "rumpo", "rumpo2", "oracle2", "oracle", "ninef2", "ninef", "minivan", "gburrito", "emperor2", "emperor"};
        public SlowDriver()
        {

            Random rnd = new Random();
            float offsetX = rnd.Next(100, 700);
            float offsetY = rnd.Next(100, 700);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Slow Driver";
            CalloutDescription = "A car is driving at very slow speeds causing traffic jams.";
            ResponseCode = 3;
            StartDistance = 250f;
        }

        public async override void OnStart(Ped player)
        {
            base.OnStart(player);
            driver.Task.CruiseWithVehicle(car, 2f, 387);
            car.AttachBlip();
            driver.AttachBlip();
            API.AddBlipForEntity(car.GetHashCode());
            API.AddBlipForEntity(driver.GetHashCode());
            PedData data1 = await Utilities.GetPedData(driver.NetworkId);
            string firstname = data1.FirstName;
            API.Wait(6000);
            DrawSubtitle("~r~[" + firstname + "] ~s~Is that a bird? Wait... I think it's a car...", 5000);
        }
        
        List<object> items = new List<object>();

      
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            Random random = new Random();
            string cartype = carList[random.Next(carList.Length)];
            VehicleHash Hash = (VehicleHash) API.GetHashKey(cartype);
            car = await SpawnVehicle(Hash, Location);
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location + 2);
            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            //Driver Data
            PedData data = new PedData();
            data.BloodAlcoholLevel = 0.05;
            List<Item> items = data.Items;
            Item Meth = new Item {
                Name = "Bag of Meth",
                IsIllegal = true
            };
            items.Add(Meth);
            Random random3 = new Random();
            string name2 = goodItemList[random3.Next(goodItemList.Length)];
            Item goodItem = new Item {
                Name = name2,
                IsIllegal = false
            };
            items.Add(goodItem);
            data.Items = items;
            Utilities.SetPedData(driver.NetworkId,data);
            
            driver.AlwaysKeepTask = true;
            driver.BlockPermanentEvents = true;
            
            PlayerData playerData = Utilities.GetPlayerData();
            string displayName = playerData.DisplayName;
            VehicleData datacar = await Utilities.GetVehicleData(car.NetworkId);
            string vehicleName = datacar.Name;
            Notify("~r~[CarCallouts] ~y~Officer ~b~" + displayName + ",~y~ the suspects are driving a " + vehicleName + "!");
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