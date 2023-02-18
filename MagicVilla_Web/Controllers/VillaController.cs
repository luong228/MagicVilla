﻿using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> listVilla = new();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                listVilla = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }

            return View(listVilla);
        }
        [Authorize(Roles = "admin")]
        public  IActionResult CreateVilla()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO model)
        {
            if(ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess == true)
                {
                    TempData["success"] = "Villa created successfully";
                    return RedirectToAction(nameof(IndexVilla));
                } 
            }
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                VillaDTO villa = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDTO>(villa));
            }
            TempData["success"] = "Error encountered!";
            return NotFound();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(model, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess == true)
                {
                    TempData["success"] = "Villa updated successfully";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            TempData["success"] = "Error encountered!";
            return View(model);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));
            if (response != null && response.IsSuccess)
            {
                VillaDTO villa = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
                return View(villa);
            }
            return NotFound();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla(VillaDTO model)
        {

                var response = await _villaService.DeleteAsync<APIResponse>(model.Id, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess == true)
                {
                TempData["success"] = "Villa deleted successfully";
                return RedirectToAction(nameof(IndexVilla));
                }
            TempData["success"] = "Error encountered!";
            return View(model);
        }
    }
}
