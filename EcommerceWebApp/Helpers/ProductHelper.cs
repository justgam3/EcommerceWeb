using EcommerceWebApi.Models;
using EcommerceWebApp.InputModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EcommerceWebApp.Helpers
{
    public class ProductHelper
    {
        //public static void UploadProductImages(ProductCreateInputModel inputModel, Product product)
        //{
        //    List<ProductImage> listImages = new List<ProductImage>();
        //    foreach (IFormFile photo in inputModel.Photos)
        //    {
        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photo.FileName);
        //        var stream = new FileStream(path, FileMode.Create);
        //        photo.CopyTo(stream);
        //        listImages.Add(new ProductImage
        //        {
        //            Path = photo.FileName,
        //        });
        //    }
        //    product.ProductImages = listImages;
        //}

        //public static void AddProductCategories(ProductCreateInputModel inputModel, Product product)
        //{
        //    List<ProductCategory> listCategories = new List<ProductCategory>();
        //    foreach (int category in inputModel.SelectedCategory)
        //    {
        //        listCategories.Add(new ProductCategory
        //        {
        //            CategoryID = category,
        //        });
        //    }
        //    product.ProductCategories = listCategories;
        //}

        //public static void AddProductVariants(ProductCreateInputModel inputModel, Product product)
        //{
        //    List<Variant> listVariants = new List<Variant>();
        //    for (int i = 0; i < inputModel.Types.Length; i++)
        //    {
        //        listVariants.Add(new Variant
        //        {
        //            Type = inputModel.Types[i],
        //            Stock = inputModel.Stocks[i],
        //        });
        //    }
        //    product.Variants = listVariants;
        //}
    }
}
