using EcommerceWebApi.Models;
using EcommerceWebApp.InputModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApp.Helpers
{
    public class ProductImageHelper
    {
        //public static List<ProductImage> UploadProductImage(ProductImageInputModel inputModel)
        //{
        //    Debug.Assert(inputModel.Photo.Length > 0);
        //    //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", inputModel.Photo.FileName);
        //    //var stream = new FileStream(path, FileMode.Create);
        //    //inputModel.Photo.CopyTo(stream);
        //    //productImage.Path = inputModel.Photo.FileName;

        //    List<ProductImage> listImages = new List<ProductImage>();
        //    foreach (IFormFile photo in inputModel.Photo)
        //    {
        //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photo.FileName);
        //        var stream = new FileStream(path, FileMode.Create);
        //        photo.CopyTo(stream);
        //        listImages.Add(new ProductImage
        //        {
        //            ProductID = inputModel.ProductID,
        //            Path = photo.FileName
        //        });
        //        stream.Close();
        //    }

        //    return listImages;
        //}
    }
}
