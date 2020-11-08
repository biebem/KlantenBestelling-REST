﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainLayer
{
    public class Client
    {
        public int Id { get; set; }
        private string _name;
        public string Name { get => _name; set { if (string.IsNullOrEmpty(value)) { throw new ArgumentException(); } _name = value; } }
        private string _address;
        public string Address { get => _address; set { if (value.Length >= 10) { throw new ArgumentException(); } _address = value; } }
        public HashSet<Order> Orders { private get; set; } = new HashSet<Order>();

        public Client(string name, string address)
        {
            Name = name;
            Address = address;
        }

        public void AddOrder(Order order)
        {
            if (order.Client == this)
            {
                if (!Orders.Add(order)) 
                {
                    Orders.TryGetValue(order, out Order orderInSet);
                    orderInSet.Amount = orderInSet.Amount + order.Amount;
                }
            }
            else
            {
                throw new ArgumentException("client is not the same.");
            }
        }
        public IReadOnlyList<Order> GetOrders()
        {
            return Orders.ToList().AsReadOnly();
        }

        public override bool Equals(object obj)
        {
            return obj is Client client &&
                   _name == client._name &&
                   _address == client._address;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_name, _address);
        }

        public override string ToString()
        {
            return $"ID: {Id} , Name: {Name}, Address: {Address}, Orders: {Orders.Count}";
        }
    }
}