using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OutlineVpn;

public class OutlineApi
{
    private WebClient _webClient = new();
    public string ApiUrl;
    public OutlineApi(string apiUrl)
    {
        ApiUrl = apiUrl;
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    }

    private bool CallRequest(string url, string method, NameValueCollection args, out string? content)
    {
        try
        {
            content = Encoding.ASCII.GetString(_webClient.UploadValues($"{ApiUrl}/{url}", method, args));
            return true;
        }
        catch
        {
            content = null;
        }
        
        return false;
    }
    
    private bool CallRequest(string url, string method, JObject args, out string? content)
    {
        try
        {
            _webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

            content = _webClient.UploadString($"{ApiUrl}/{url}", method, args.ToString());
            return true;
        }
        catch
        {
            content = null;
        }
        
        return false;
    }
    
    public List<OutlineKey> GetKeys()
    {
        CallRequest("access-keys", "GET", new NameValueCollection(), out string? content);
        return (JObject.Parse(content)["accessKeys"] as JArray).ToObject<List<OutlineKey>>();
    }

    public OutlineKey CreateKey()
    {
        CallRequest("access-keys", "POST", new NameValueCollection(), out string? content);
        return (JObject.Parse(content)).ToObject<OutlineKey>();
    }

    public bool DeleteKey(int id)
        => CallRequest($"access-keys/{id}", "DELETE", new NameValueCollection(), out _);

    public bool RenameKey(int id, string name)
        => CallRequest($"access-keys/{id}/name", "PUT", new JObject
        {
            {"name", name}
        }, out _);

    public bool AddDataLimit(int id, long limitBytes)
        => CallRequest($"access-keys/{id}/data-limit", "PUT", new JObject
        {
            {
                "limit", new JObject
                {
                    {"bytes", limitBytes}
                }
            }
        }, out _);

    public bool DeleteDataLimit(int id)
        => CallRequest($"access-keys/{id}/data-limit", "DELETE", new NameValueCollection(), out _);

    public List<OutlineKey> GetTransferredData()
    {
        List<OutlineKey> outline = new List<OutlineKey>();
        CallRequest("metrics/transfer", "GET", new NameValueCollection(), out string content);
        var response = JObject.Parse(content)["bytesTransferredByUserId"] as JObject;
        foreach (var x in response)
        {
            OutlineKey outl = new OutlineKey()
            {
                Id = int.Parse(x.Key),
                UsedBytes = (long)x.Value
            };
            outline.Add(outl);
        }
        return outline;
    }
}