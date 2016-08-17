using System;
using System.Collections.Generic;
using System.Net;

/**
 * This client serves as a basic model of how to interact with our api in C#. It is
 * limited to our /companies endpoint and does not perform any client side validation
 * of inputs. For further information regarding the capabilities of the api,
 * please visit: https://ycharts.com/api/docs/
 */
public class YChartsApiClient
{
    internal string base_url;
    internal string api_key;

    public YChartsApiClient(string _api_key)
    {
        base_url = "https://ycharts.com/api/v3/";
        api_key = _api_key;
    }

    /**
     * Get company infos for given tickers and info_fields
     * @param tickers list of symbols for companies
     * @param info_fields list of requested info fields
     * @return a string in JSON format of the requested info fields for the companies
     */
    public virtual string get_company_info(List<string> tickers, List<string> info_fields)
    {
        string url = base_url + "companies/" + String.Join(",", tickers) + "/info/" + String.Join(",", info_fields);
        string json_data = _get_data(url);
        return json_data;
    }

    /**
     * Get company data points for given tickers, metrics, and date
     * @param tickers list of symbols for companies
     * @param metrics list of requested financial metrics
     * @param date string in the YYYY-MM-DD format
     * @return a string in JSON format of the requested point data for the companies
     */
    public virtual string get_company_data_point(List<string> tickers, List<string> metrics, string date)
    {
        string url = base_url + "companies/" + String.Join(",", tickers) + "/points/" + String.Join(",", metrics);

        string @params = "";
        if (!date.Equals(""))
        {
            @params = "date=" + System.Uri.EscapeDataString(date);
        }
        string json_data = _get_data(url, @params);
        return json_data;
    }

    /**
     * Get company data series for given tickers, metrics, start_date, and end_date
     * @param tickers list of symbols for companies
     * @param metrics list of requested financial metrics
     * @param start_date string in the YYYY-MM-DD format representing start date
     * @param end_date string in the YYYY-MM-DD format representing end date
     * @return a string in JSON format of the requested series data for the companies
     */
    public virtual string get_company_data_timeseries(List<string> tickers, List<string> metrics,
        string start_date, string end_date)
    {
        string url = base_url + "companies/" + String.Join(",", tickers) + "/series/" + String.Join(",", metrics);

        string @params = "";
        if (!start_date.Equals(""))
        {
            @params += "start_date=" + System.Uri.EscapeDataString(start_date);
        }
        if (!end_date.Equals(""))
        {
            @params += "&end_date=" + System.Uri.EscapeDataString(end_date);
        }
        string json_data = _get_data(url, @params);
        return json_data;
    }

    /**
     * Get json response from server for the url string
     * @param url_str the requested url
     * @return a string in JSON format of the requested url
     */
    public virtual string _get_data(string url_str)
    {
        return _get_data(url_str, "");
    }

    /**
     * Get json response from server for the url string and params
     * @param url_str the requested url
     * @param @params the query string parameters
     * @return a string in JSON format of the requested url
     */
    public virtual string _get_data(string url_str, string @params)
    {
        if (!@params.Equals(""))
        {
            url_str = url_str + "?" + @params;
        }

        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url_str);
        request.ContentType = "application/json";
        request.Headers.Add("X-YCHARTSAUTHORIZATION", api_key);
        request.Method = "GET";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
        {
            return reader.ReadToEnd();
        }
    }

    public static void Main(string[] args)
    {
        string api_key = "";  // Enter the Key here. See https://ycharts.com/accounts/my_account

        YChartsApiClient client = new YChartsApiClient(api_key);

        string info_rsp = client.get_company_info(
            new List<string> { "AAPL", "MSFT" }, new List<string> { "exchange", "industry" });

        string points_rsp = client.get_company_data_point(
            new List<string> { "AAPL", "MSFT" }, new List<string> { "price", "pe_ratio" }, "2016-03-03");

        string series_rsp = client.get_company_data_timeseries(
            new List<string> { "AAPL", "MSFT" }, new List<string> { "price" }, "2016-03-03", "2016-03-15");

        //print to file
        string[] lines = {
            info_rsp,
            points_rsp,
            series_rsp
        };

        System.IO.File.WriteAllLines(@"c#_test_client_output.txt", lines);
    }
}
