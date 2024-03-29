﻿using GolfAPI.Core.Contracts.Api;
using GolfAPI.Core.Contracts.DataAccess;
using GolfAPI.Core.Contracts.Managers;
using GolfAPI.DataLayer.DataAccess;
using GolfAPI.DataLayer.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GolfAPI.Core.Managers
{
    public class ComponentOrderManager : IComponentOrderManager
    {
        private readonly IComponentRepository _repository;
        public ComponentOrderManager(
            IComponentRepository repository
            )
        {
            _repository = repository;
        }

        public IEnumerable<OrderComponent> BuildComponentsOrder(int orderId, ComponentApi[] components)
        {
            var result = new List<OrderComponent>();

            if (components != null && 0 < components.Count())
            {
                foreach (var component in components)
                {
                    var com = _repository.GetComponentByCode(component.ComponentCode);
                    if (com == null)
                    {
                        com = new Component()
                        {
                            ComponentCode = component.ComponentCode
                        };
                    }
                    result.Add(new OrderComponent()
                    {
                        OrderId = orderId,
                        Component = com,
                        ComponentQuantity = component.Quantity
                    });


                }

            }
            return result;
        }

        public async Task<int> UpdateOrderComponents(int orderId, ComponentApi[] componentQuantity)
        {
            var dbComponents = _repository.GetByOrderId(orderId);
            
            foreach (var component in componentQuantity)
            {
                //get the component with same code for the order
                var dbComponent = dbComponents.Keys.Where(c=> c.ComponentCode == component.ComponentCode)
                    .FirstOrDefault();
                //if exist -> update
                if (dbComponent != null)
                {
                    try
                    {
                        var resultOp = await _repository.TryUpdateComponent(orderId, 
                            dbComponent, 
                            component.Quantity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return -1;

                    }
                }
                else
                {
                    //else check if exist a component with the same code for all the orders
                    var com = _repository.GetComponentByCode(component.ComponentCode);
                    //if exist, we add a relationship between this component 
                    // and the order
                    //if not create a new component
                    if (com == null)
                    {
                        com = new Component()
                        {
                            ComponentCode = component.ComponentCode
                        };
                    }

                    var newComponent = new OrderComponent()
                    {
                        OrderId = orderId,
                        Component = com,
                        ComponentQuantity = component.Quantity
                    };
                    await _repository.AddNewComponentToOrder(newComponent);
                }
            }

            var deletedTask = DeleteOldComponentsFromOrder(orderId, componentQuantity);
            
            return 0;

        }


        private async Task<int> DeleteOldComponentsFromOrder(int orderId, ComponentApi[] newComponents )
        {
            var componentCodes = (from cq in newComponents select cq.ComponentCode).ToList();
            var dbComponents = _repository.GetByOrderId(orderId);

            var dbComponentsCodes = from c in dbComponents.Keys select c.ComponentCode;
            var componentsToDelete = dbComponentsCodes.Where(c => !componentCodes.Contains(c));

            try
            {
                await _repository.DeleteComponentsFromOrder(orderId, componentsToDelete.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
            return 0;
        }
        
    }
}
