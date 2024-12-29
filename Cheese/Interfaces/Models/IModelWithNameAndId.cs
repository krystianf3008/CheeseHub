﻿namespace CheeseHub.Interfaces.Models
{
    //interface helps build abstract service
    public interface IModelWithNameAndId
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
