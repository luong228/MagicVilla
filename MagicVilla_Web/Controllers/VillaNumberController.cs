using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> listVillaNumber = new();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                listVillaNumber = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }

            return View(listVillaNumber);
        }
        public  IActionResult CreateVillaNumber()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateDTO model)
        {
            if(ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess == true)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                } 
            }
            return View(model);
        }
        public async Task<IActionResult> UpdateVillaNumber(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO villa = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaNumberUpdateDTO>(villa));
            }
            return NotFound();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess == true)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteVillaNumber(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO villa = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                return View(villa);
            }
            return NotFound();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDTO model)
        {

                var response = await _villaService.DeleteAsync<APIResponse>(model.VillaNO);
                if (response != null && response.IsSuccess == true)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
            return View(model);
        }
    }
}
