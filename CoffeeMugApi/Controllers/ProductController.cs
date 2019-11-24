using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoffeeMugApi.DL.Services.ProductService;
using CoffeeMugApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CoffeeMugApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        private IMapper _mapper;

        public ProductController(IProductService productsService, IMapper mapper)
        {
            this._productService = productsService;
            this._mapper = mapper;
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            var dtoProducts = await this._productService.List();
            var apiProducts = this._mapper.Map<IEnumerable<DA.DtoModels.Product>, IEnumerable<Models.Product>>(dtoProducts);
            return Ok(apiProducts);
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            if (ModelState.IsValid)
            {
                var dtoProduct = await this._productService.GetProductById(id);
                if (dtoProduct == null)
                    return NotFound(id);
                else
                {
                    var apiProduct = this._mapper.Map<DA.DtoModels.Product, Models.Product>(dtoProduct);
                    return Ok(apiProduct);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ResponseCache(NoStore = true, Duration = 0)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var dtoProduct = this._mapper.Map<Models.Product, DA.DtoModels.Product>(product);
                var id = await this._productService.AddProduct(dtoProduct);
                var url = Url.Action("Get", "PicturesController", new { id = id }, Request.Scheme);
                return Created($"{Request.Path}/{id.ToString()}", id);
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("{id}")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (ModelState.IsValid)
            {
                var dtoProduct = this._mapper.Map<Models.Product, DA.DtoModels.Product>(product);
                var result = await this._productService.UpdateProduct(id, dtoProduct);

                if (result)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("{id}")]
        [ResponseCache(NoStore = true, Duration = 0)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if (ModelState.IsValid)
            {
                var result = await this._productService.RemoveProduct(id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest(ModelState);
        }
    }
}

