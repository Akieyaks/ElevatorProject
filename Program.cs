using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

enum Direction
{
    Up,
    Down,
    None
}

class Elevator
{
    private int currentFloor; // Change to private field

    public int CurrentFloor
    {
        get { return currentFloor; }
    }

    public Direction Direction { get; private set; }
    public bool IsMoving { get; private set; }
    public int PassengerCount { get; private set; }
    public int MaxCapacity { get; private set; }

    public Elevator(int maxCapacity)
    {
        currentFloor = 0; // Start from floor 0
        Direction = Direction.None;
        IsMoving = false;
        PassengerCount = 0;
        MaxCapacity = maxCapacity;
    }

    public void MoveToFloor(int targetFloor)
    {
        Direction = targetFloor > currentFloor ? Direction.Up : Direction.Down;
        IsMoving = true;

        while (currentFloor != targetFloor)
        {
            Thread.Sleep(1000); // Simulating movement delay
            currentFloor += Direction == Direction.Up ? 1 : -1;
            Console.WriteLine($"Elevator is moving to floor {currentFloor}");
        }

        IsMoving = false;
        OpenDoors();
    }

    public void OpenDoors()
    {
        Console.WriteLine("Elevator doors opened.");
        // Additional logic for opening doors
    }

    public void CloseDoors()
    {
        Console.WriteLine("Elevator doors closed.");
        // Additional logic for closing doors
    }

    public void LoadPassengers(int passengers)
    {
        if (PassengerCount + passengers > MaxCapacity)
        {
            Console.WriteLine("Elevator is overloaded. Unable to load passengers.");
            return;
        }

        PassengerCount += passengers;
        Console.WriteLine($"{passengers} passengers loaded. Current passenger count: {PassengerCount}");
    }

    public void UnloadPassengers(int passengers)
    {
        PassengerCount -= passengers;
        Console.WriteLine($"{passengers} passengers unloaded. Current passenger count: {PassengerCount}");
    }

    public void DisplayStatus()
    {
        Console.WriteLine($"Elevator at floor {currentFloor}, Direction: {Direction}, Moving: {IsMoving}, Passenger Count: {PassengerCount}");
    }

    public void UpdateCurrentFloor(int newFloor)
    {
        currentFloor = newFloor;
    }
}

class Building
{
    private List<Elevator> elevators;

    public Building(int numberOfFloors, int numberOfElevators)
    {
        elevators = new List<Elevator>();

        for (int i = 0; i < numberOfElevators; i++)
        {
            int elevatorCapacity = CalculateElevatorCapacity(numberOfFloors, numberOfElevators);
            elevators.Add(new Elevator(elevatorCapacity));
        }
    }

    private int CalculateElevatorCapacity(int numberOfFloors, int numberOfElevators)
    {
        // Calculate the elevator capacity based on the total number of floors and elevators
        // You can adjust this logic based on your requirements
        int totalCapacity = numberOfFloors * 10; // Assuming 10 people per floor as a base capacity
        int elevatorCapacity = totalCapacity / numberOfElevators;

        return elevatorCapacity;
    }

    public Elevator RequestElevator(int currentFloor)
    {
        // Logic to dispatch the nearest available elevator
        Elevator nearestElevator = elevators[0];
        int minDistance = Math.Abs(nearestElevator.CurrentFloor - currentFloor);

        for (int i = 1; i < elevators.Count; i++)
        {
            int distance = Math.Abs(elevators[i].CurrentFloor - currentFloor);

            if (distance < minDistance)
            {
                nearestElevator = elevators[i];
                minDistance = distance;
            }
        }

        return nearestElevator;
    }

    public List<Elevator> GetElevators()
    {
        return elevators;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the number of floors in the building: ");
        int numberOfFloors = GetValidInput();

        Console.WriteLine("Enter the number of elevators: ");
        int numberOfElevators = GetValidInput();

        Building building = new Building(numberOfFloors, numberOfElevators);

        Console.WriteLine("Elevator Limits:");
        foreach (Elevator elevator in building.GetElevators())
        {
            Console.WriteLine($"Elevator: {elevator}, Limit: {elevator.MaxCapacity}");
        }

        while (true)
        {
            Console.WriteLine("Enter your current floor: ");
            int currentFloor = GetValidInput();

            // Validate if the current floor is within the valid range
            if (currentFloor < 1 || currentFloor > numberOfFloors)
            {
                Console.WriteLine($"Invalid current floor. Please enter a floor between 1 and {numberOfFloors}.");
                continue; // Restart the loop
            }

            Console.WriteLine("Enter the number of people waiting for the elevator: ");
            int peopleCount = GetValidInput();

            Console.WriteLine("Enter the destination floor: ");
            int targetFloor = GetValidInput();

            // Validate if the target floor is within the valid range
            if (targetFloor < 1 || targetFloor > numberOfFloors)
            {
                Console.WriteLine($"Invalid target floor. Please enter a floor between 1 and {numberOfFloors}.");
                continue; // Restart the loop
            }

            Elevator selectedElevator = building.RequestElevator(currentFloor);

            Console.WriteLine($"Elevator {selectedElevator} dispatched to floor {currentFloor}");

            // Show the movement of the elevator to the called floor
            ShowElevatorMovement(selectedElevator, currentFloor);

            selectedElevator.LoadPassengers(peopleCount);

            // Show the movement of the elevator to the destination floor
            ShowElevatorMovement(selectedElevator, targetFloor);

            selectedElevator.MoveToFloor(targetFloor);
            selectedElevator.UnloadPassengers(peopleCount);

            // Display the status of all elevators after each trip
            Console.WriteLine("Elevator Status:");
            foreach (Elevator elevator in building.GetElevators())
            {
                elevator.DisplayStatus();
            }
        }
    }

    static void ShowElevatorMovement(Elevator elevator, int targetFloor)
    {
        Console.WriteLine($"Elevator {elevator} is moving to floor {targetFloor}");
        // Simulate movement delay
        Thread.Sleep(1000);
        // Update the elevator's current floor using the public method
        elevator.UpdateCurrentFloor(targetFloor);
        Console.WriteLine($"Elevator {elevator} has arrived at floor {targetFloor}");
    }

    static int GetValidInput()
    {
        int input;
        while (!int.TryParse(Console.ReadLine(), out input) || input <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive integer.");
        }
        return input;
    }
}

[TestClass]
public class ElevatorTests
{
    [TestMethod]
    public void MoveToFloor_ShouldUpdateCurrentFloor()
    {
        // Arrange
        Elevator elevator = new Elevator(10);

        // Act
        elevator.MoveToFloor(5);

        // Assert
        Assert.AreEqual(5, elevator.CurrentFloor);
    }

    [TestMethod]
    public void LoadPassengers_ShouldUpdatePassengerCount()
    {
        // Arrange
        Elevator elevator = new Elevator(10);

        // Act
        elevator.LoadPassengers(3);

        // Assert
        Assert.AreEqual(3, elevator.PassengerCount);
    }
}

[TestClass]
public class BuildingTests
{
    [TestMethod]
    public void RequestElevator_ShouldReturnNearestElevator()
    {
        // Arrange
        Building building = new Building(10, 3);

        // Act
        Elevator elevator = building.RequestElevator(3);

        // Assert
        Assert.IsNotNull(elevator);
    }
}
