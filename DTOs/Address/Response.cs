namespace FoodDelivery.DTOs;
public class AddressResponse
{
    public Guid Id {get;set;}
    public string? FullAddress {get;set;}
    public string? Label {get;set;}
    public bool IsDefault {get;set;}
    public double Latitude {get;set;}
    public double Longitude {get;set;}

}

public class AddAddressResponse
{
    public Guid Id {get;set;}
    public string FullAddress {get;set;} = string.Empty;
    public string Label {get;set;} = string.Empty;
    public bool IsDefault {get;set;}
    public double Latitude {get;set;}
    public double Longitude {get;set;}
    public DateTime CreateAt {get;set;}
}