using DataAccess.Abstract;
using Entities.Concreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concreate.InMemory
{    public class InMemoryProductDal: ICarDal
    {
        List<Car> _cars;

        public InMemoryProductDal()
        {
            _cars = new List<Car>
            {
                new Car { Id = 1, BrandId = 1, ColorId =1, DailyPrice = 500, ModelYear = "2009", Description = "BMW Benzinli" },
                new Car { Id = 2, BrandId = 1, ColorId =3, DailyPrice = 1500, ModelYear = "2019", Description = "Mercedes LPG" },
                new Car { Id = 3, BrandId = 2, ColorId =4, DailyPrice = 2500, ModelYear = "2022", Description = "Audi LPG" },
                new Car { Id = 4, BrandId = 3, ColorId =2, DailyPrice = 3500, ModelYear = "2023", Description = "BMW Benzinli" },
            };
        }

        public void Add(Car car)
        {
            _cars.Add(car);
        }

        public void Delete(Car car)
        {
            Car carToDelete = _cars.FirstOrDefault(c => c.Id == car.Id);
            _cars.Remove(carToDelete);
        }

        public List<Car> GetAll()
        {
            return _cars.ToList();
        }

        public List<Car> GetById(int id)
        {
           return _cars.Where(c => c.Id == id).ToList();
        }

        public void Update(Car car)
        {
            Car updatedCar = _cars.FirstOrDefault(c => c.Id == car.Id);
            updatedCar.Id = car.Id;
            updatedCar.BrandId = car.BrandId;
            updatedCar.ColorId = car.ColorId;
            updatedCar.DailyPrice = car.DailyPrice;
            updatedCar.Description = car.Description;
            updatedCar.ModelYear = car.ModelYear;
        }
    }
}
