using Hotel.Application.Common.InterFaces;
using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace Hotel.Application.Services.Implementation
{
    public class VillaServices(IUnitOfWork unit, IWebHostEnvironment webHost) : IVillaServices
    {
        public IEnumerable<Villa> GetAllVilla(string? includeProp = null)
        {
            return unit.VillaRepository.GetAllAsync(includeProperty: includeProp);
        }
        public async Task<Villa> GetVillaById(int id, string? includeProp1 = null)
        {
            return await unit.VillaRepository.GetAsync(x => x.Id == id, includeProperty: includeProp1);
        }

        public async Task CreateAsync(Villa villa)
        {
            if (villa.Image is not null)
            {
                string fileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                string imagePath = Path.Combine(webHost.WebRootPath, @"Images\Villa");
                await using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                await villa.Image.CopyToAsync(fileStream);
                villa.ImageUrl = @"\Images\Villa\" + fileName;
            }
            else
            {
                villa.ImageUrl = "https://placehold.co/600x400"; //default image
            }
            await unit.VillaRepository.AddAsync(villa);
        }

        public async Task UpdateAsync(Villa villa)
        {
            if (villa.Image is not null)
            {
                string fileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                string imagePath = Path.Combine(webHost.WebRootPath, @"Images\Villa");
                if (!string.IsNullOrEmpty(villa.ImageUrl))
                {
                    string oldImagePath = Path.Combine(webHost.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                await using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                await villa.Image.CopyToAsync(fileStream);
                villa.ImageUrl = @"\Images\Villa\" + fileName;
            }
            await unit.VillaRepository.UpdateAsync(villa);
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var villaObj = await unit.VillaRepository.GetAsync(x => x.Id == id);

                if (villaObj is not null)
                {
                    if (!string.IsNullOrEmpty(villaObj.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(webHost.WebRootPath, villaObj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    await unit.VillaRepository.RemoveAsync(villaObj);
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Villa> GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = unit.VillaRepository.GetAllAsync(includeProperty: "VillaAmenities").ToList();
            var villaNumberList = unit.VillaNumberRepository.GetAllAsync().ToList();
            var bookedVillas = unit.BookingRepository.GetAllAsync(x => x.Status == SD.StatusApproved
                                                                       || x.Status == SD.StatusCheckedIn).ToList();
            foreach (var villa in villaList)
            {
                int roomAvailable =
                    SD.VillaRoomsAvailable_Count(villa.Id, villaNumberList, checkInDate, nights, bookedVillas);
                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }

            return villaList;
        }

        public bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate)
        {
            var villaNumbersList = unit.VillaNumberRepository.GetAllAsync().ToList();
            var bookedVillas = unit.BookingRepository.GetAllAsync(u => u.Status == SD.StatusApproved ||
                                                               u.Status == SD.StatusCheckedIn).ToList();

            int roomAvailable = SD.VillaRoomsAvailable_Count
                (villaId, villaNumbersList, checkInDate, nights, bookedVillas);

            return roomAvailable > 0;
        }
    }
}
