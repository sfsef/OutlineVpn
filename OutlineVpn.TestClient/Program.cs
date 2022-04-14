using OutlineVpn;

var api = new OutlineApi("MANAGEMENT API URL HERE"); 

var data = api.CreateKey(); // Create new key
api.RenameKey(data.Id, "Test_name"); // Rename new key

var data2 = api.GetTransferredData(); // Get all transferred data

Console.WriteLine(data2.FirstOrDefault(k => k.Id == 0).UsedBytes); // Print used traffic with id 0



