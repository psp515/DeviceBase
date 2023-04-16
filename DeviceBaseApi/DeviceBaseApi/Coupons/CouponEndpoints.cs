using AutoMapper;
using FluentValidation;
using DeviceBaseApi.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using DeviceBaseApi.Coupons.DTO;

namespace DeviceBaseApi.Coupons;

public static class CouponEndpoints
{
    public static void ConfigureCouponEndpoints(this WebApplication app)
    {

        app.MapGet("/api/coupon", GetAllCoupon)
            .WithName("GetCoupons")
            .Produces<RestResponse>(200);

        app.MapGet("/api/coupon/{id:int}", GetCoupon)
            .WithName("GetCoupon")
            .Produces<RestResponse>(200);

        app.MapPost("/api/coupon", CreateCoupon)
            .WithName("CreateCoupon")
            .Accepts<CouponCreateDTO>("application/json")
            .Produces<RestResponse>(201)
            .Produces(400);

        app.MapPut("/api/coupon", UpdateCoupon)
            .WithName("UpdateCoupon")
            .Accepts<CouponUpdateDTO>("application/json")
            .Produces<RestResponse>(200)
            .Produces(400);

        app.MapDelete("/api/coupon/{id:int}", DeleteCoupon);
    }

    private async static Task<IResult> GetCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger, int id)
    {
        var result = await _couponRepo.GetAsync(id);
        return Results.Ok(new RestResponse(HttpStatusCode.OK, true, result));
    }

    private async static Task<IResult> CreateCoupon(ICouponRepository _couponRepo, IMapper _mapper, [FromBody] CouponCreateDTO coupon_C_DTO)
    {

        if (_couponRepo.GetAsync(coupon_C_DTO.Name).GetAwaiter().GetResult() != null)
            return Results.BadRequest(new RestResponse("Coupon Name already Exists"));


        Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

        await _couponRepo.CreateAsync(coupon);
        await _couponRepo.SaveAsync();
        CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

        return Results.Ok(new RestResponse(HttpStatusCode.Created, true, couponDTO));
        //return Results.CreatedAtRoute("GetCoupon",new { id=coupon.Id }, couponDTO);
        //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
    }

    private async static Task<IResult> UpdateCoupon(ICouponRepository _couponRepo, IMapper _mapper,
             [FromBody] CouponUpdateDTO coupon_U_DTO)
    {
        RestResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


        await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(coupon_U_DTO));
        await _couponRepo.SaveAsync();

        response.Result = _mapper.Map<CouponDTO>(await _couponRepo.GetAsync(coupon_U_DTO.Id)); ;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private async static Task<IResult> DeleteCoupon(ICouponRepository _couponRepo, int id)
    {
        RestResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


        Coupon couponFromStore = await _couponRepo.GetAsync(id);
        if (couponFromStore != null)
        {
            await _couponRepo.RemoveAsync(couponFromStore);
            await _couponRepo.SaveAsync();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(response);
        }
        else
        {
            return Results.BadRequest(new RestResponse("Invalid Id"));
        }
    }

    private async static Task<IResult> GetAllCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger)
    {
        RestResponse response = new();
        _logger.Log(LogLevel.Information, "Getting all Coupons");
        response.Result = await _couponRepo.GetAllAsync();
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
}
