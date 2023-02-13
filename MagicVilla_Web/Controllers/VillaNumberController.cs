using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.ViewModels;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
            _villaService = villaService;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> listVillaNumber = new();
            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                listVillaNumber = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }

            return View(listVillaNumber);
        }
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberVM = new(); 
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)).Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            return View(villaNumberVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if(ModelState.IsValid)
            {
                if (model.VillaNumber.VillaNO == 0)
                {
                    ModelState.AddModelError("ErrorMessages", "VillaNO couldn't not be zero");
                }
                else
                {
                    var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                    if (response != null && response.IsSuccess == true)
                    {
                        return RedirectToAction(nameof(IndexVillaNumber));
                    }
                    else
                    {
                        if (response.ErrorMessages.Count > 0)
                        {
                            ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                        }
                    }
                }
            }
            //Get List Villa For Select Item
            var responseAllVilla = await _villaService.GetAllAsync<APIResponse>();
            if (responseAllVilla != null && responseAllVilla.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responseAllVilla.Result)).Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            return View(model);
        }
        public async Task<IActionResult> UpdateVillaNumber(int id)
        {
            VillaNumberUpdateVM villaNumberVM = new();

            var response = await _villaNumberService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);

                //Get List Villa For Select Item
                var responseAllVilla = await _villaService.GetAllAsync<APIResponse>();
                if (responseAllVilla != null && responseAllVilla.IsSuccess)
                {
                    villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responseAllVilla.Result)).Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                }
                return View(villaNumberVM);
            }

            return NotFound();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess == true)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }
            // Response Villa Select List If there is any Error
            var responseGetAll = await _villaService.GetAllAsync<APIResponse>();
            if (responseGetAll != null && responseGetAll.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responseGetAll.Result)).Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteVillaNumber(int id)
        {
            VillaNumberDeleteVM villaNumberVM = new();

            var response = await _villaNumberService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = villaNumber;

                //Get List Villa For Select Item
                var responseAllVilla = await _villaService.GetAllAsync<APIResponse>();
                if (responseAllVilla != null && responseAllVilla.IsSuccess)
                {
                    villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(responseAllVilla.Result)).Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                }
                return View(villaNumberVM);
            }

            return NotFound();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {

                var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNO);
                if (response != null && response.IsSuccess == true)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
            return View(model);
        }
    }
}
