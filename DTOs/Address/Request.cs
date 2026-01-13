namespace FoodDelivery.DTOs;
public class AddressRequest
{
    public string FullAddress {get;set;} = string.Empty;
    public string Label {get;set;} = string.Empty;
    public bool IsDefault {get;set;}
    public double Latitude {get;set;}
    public double Longitude {get;set;}
}
public class AddAddressRequest
{
    public string FullAddress {get;set;} = string.Empty;
    public string Label {get;set;} = string.Empty;
    public double Latitude {get;set;}
    public double Longitude {get;set;}
    public bool IsDefault {get;set;}
}