﻿using MagicVilla_Web.Models;

namespace MagicVilla_Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse responseModel { get; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
