using System;
using System.Collections.Generic;
using System.Linq; // Required for LINQ extensions like .Where() and .Any()

// Custom exception for calculation failures, inheriting from Exception
public class CalculationException : Exception
{
    // Constructor that passes the message to the base Exception class
    public CalculationException(string message) : base(message) { }

    // Optional: Constructor that also takes an inner exception for detailed error logging
    public CalculationException(string message, Exception innerException) : base(message, innerException) { }
}

public class CourseStudent
{
    // Public properties to store student information
    public string FullName { get; set; }
    public double AssignmentMark { get; set; }
    public double ExamMark { get; set; }

    /// <summary>
    /// Initializes a new instance of the CourseStudent class.
    /// Validates that assignment and exam marks are within the valid range (0-100).
    /// </summary>
    /// <param name="fullName">The full name of the student.</param>
    /// <param name="assignmentMark">The mark for the assignment (0-100).</param>
    /// <param name="examMark">The mark for the exam (0-100).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if marks are out of the 0-100 range.</exception>
    public CourseStudent(string fullName, double assignmentMark, double examMark)
    {
        FullName = fullName;

        // Validate AssignmentMark
        if (assignmentMark < 0 || assignmentMark > 100)
        {
            // Throws an ArgumentOutOfRangeException if the value is out of range
            throw new ArgumentOutOfRangeException(nameof(assignmentMark), "Assignment mark must be between 0 and 100.");
        }
        AssignmentMark = assignmentMark;

        // Validate ExamMark
        if (examMark < 0 || examMark > 100)
        {
            // Throws an ArgumentOutOfRangeException if the value is out of range
            throw new ArgumentOutOfRangeException(nameof(examMark), "Exam mark must be between 0 and 100.");
        }
        ExamMark = examMark;
    }

    /// <summary>
    /// Calculates the final mark for the student.
    /// (AssignmentMark * 0.4) + (ExamMark * 0.6)
    /// </summary>
    /// <returns>The calculated final mark.</returns>
    /// <exception cref="CalculationException">Thrown if the calculation fails due to an unexpected error.</exception>
    public double GetFinalMark()
    {
        try
        {
            // Perform the final mark calculation
            // The constructor ensures marks are valid doubles, so simple arithmetic is safe.
            // This try-catch block demonstrates the principle of catching unexpected errors.
            return (AssignmentMark * 0.4) + (ExamMark * 0.6);
        }
        catch (Exception ex)
        {
            // Catch any unexpected exceptions during calculation and re-throw as a custom exception
            throw new CalculationException("Failed to calculate final mark. An unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Returns a formatted string representation of the CourseStudent object.
    /// </summary>
    /// <returns>A string showing full name, assignment mark, exam mark, and final mark.</returns>
    public override string ToString()
    {
        double finalMark;
        try
        {
            // Attempt to get the final mark
            finalMark = GetFinalMark();
        }
        catch (CalculationException)
        {
            // If calculation fails, set finalMark to NaN (Not a Number)
            // This indicates a failed calculation without crashing the application.
            finalMark = double.NaN;
        }

        // Return a formatted string using string interpolation.
        // :F2 formats the double to two decimal places.
        return $"Full Name: {FullName}, Assignment Mark: {AssignmentMark:F2}, Exam Mark: {ExamMark:F2}, Final Mark: {finalMark:F2}";
    }
}

// 4. Delegate Usage
// Declares a delegate named StudentFilter.
// It takes a CourseStudent object as a parameter and returns a boolean.
public delegate bool StudentFilter(CourseStudent student);

class Program
{
    static void Main(string[] args)
    {
        // Create a List<CourseStudent> to store student objects
        // This demonstrates the use of a generic data structure.
        List<CourseStudent> students = new List<CourseStudent>();

        try
        {
            // Add five hardcoded student objects to the list.
            // The constructor's validation will be triggered here.
            students.Add(new CourseStudent("Alice Smith", 85.0, 90.0));
            students.Add(new CourseStudent("Bob Johnson", 70.0, 65.0));
            students.Add(new CourseStudent("Charlie Brown", 92.5, 88.0));
            students.Add(new CourseStudent("Diana Prince", 45.0, 52.0));
            students.Add(new CourseStudent("Eve Adams", 78.0, 82.0));
            // Add an extra student to easily test the "below 50%" functionality
            students.Add(new CourseStudent("Frank Miller", 30.0, 40.0));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // Catch specific exceptions related to invalid mark input
            Console.WriteLine($"Error creating student: {ex.Message}");
        }

        Console.WriteLine("--- All Student Details ---");
        // Use a foreach loop to iterate through the list and display all student details
        // The overridden ToString() method is implicitly called here.
        foreach (var student in students)
        {
            Console.WriteLine(student);
        }
        Console.WriteLine(); // Add a blank line for readability

        // 2. Lambda Expression Usage
        Console.WriteLine("--- Students with Final Mark Above 80% ---");
        // Use LINQ's .Where() method with a lambda expression to filter students.
        // s => !double.IsNaN(s.GetFinalMark()) && s.GetFinalMark() > 80
        // This lambda checks if the final mark is not NaN (i.e., successfully calculated)
        // AND if it's greater than 80.
        var highAchievers = students.Where(s => !double.IsNaN(s.GetFinalMark()) && s.GetFinalMark() > 80);
        foreach (var student in highAchievers)
        {
            Console.WriteLine(student);
        }
        Console.WriteLine();

        // 3. Static Methods Usage
        Console.WriteLine("--- Static Method Usage ---");
        // Call the static methods and display their results.
        Console.WriteLine($"Number of students above average: {CountStudentsAboveAverage(students)}");
        Console.WriteLine($"Number of students below 50%: {CountStudentsBelowFifty(students)}");
        Console.WriteLine();

        // 5. Delegate Usage in Main()
        Console.WriteLine("--- Delegate Usage: Students with Final Mark between 60% and 70% ---");
        // Assign a lambda expression to the StudentFilter delegate.
        // This lambda defines the filtering logic: final mark between 60 and 70 (inclusive).
        StudentFilter filterByRange = (student) =>
            !double.IsNaN(student.GetFinalMark()) && student.GetFinalMark() >= 60 && student.GetFinalMark() <= 70;

        // Iterate through all students and apply the filter using the delegate.
        foreach (var student in students)
        {
            if (filterByRange(student)) // Call the delegate to check the condition
            {
                Console.WriteLine(student);
            }
        }
        Console.WriteLine();

        Console.WriteLine("Press any key to exit.");
        Console.ReadLine(); // Keeps the console window open until a key is pressed.
    }

    /// <summary>
    /// Calculates the number of students whose final mark is above the average final mark of all valid students.
    /// </summary>
    /// <param name="students">The list of CourseStudent objects.</param>
    /// <returns>The count of students above average.</returns>
    public static int CountStudentsAboveAverage(List<CourseStudent> students)
    {
        // Handle null or empty list to prevent errors.
        if (students == null || !students.Any())
        {
            return 0;
        }

        double totalFinalMarks = 0;
        int validStudentsCount = 0; // Counter for students with successfully calculated marks

        // Iterate through students to sum up valid final marks and count valid students
        foreach (var student in students)
        {
            try
            {
                double finalMark = student.GetFinalMark();
                if (!double.IsNaN(finalMark)) // Only include valid marks in the average calculation
                {
                    totalFinalMarks += finalMark;
                    validStudentsCount++;
                }
            }
            catch (CalculationException)
            {
                // If a student's final mark calculation fails, they are skipped for the average.
            }
        }

        // If no valid students, the average is 0, so no one can be above it.
        if (validStudentsCount == 0)
        {
            return 0;
        }

        // Calculate the average final mark
        double average = totalFinalMarks / validStudentsCount;

        // Use LINQ's .Count() method with a lambda to count students above the average.
        return students.Count(s => {
            try
            {
                // Ensure final mark is valid before comparison
                return !double.IsNaN(s.GetFinalMark()) && s.GetFinalMark() > average;
            }
            catch (CalculationException)
            {
                // If a student's mark cannot be calculated, they don't count towards being above average.
                return false;
            }
        });
    }

    /// <summary>
    /// Calculates the number of students whose final mark is below 50%.
    /// </summary>
    /// <param name="students">The list of CourseStudent objects.</param>
    /// <returns>The count of students below 50%.</returns>
    public static int CountStudentsBelowFifty(List<CourseStudent> students)
    {
        // Handle null or empty list.
        if (students == null || !students.Any())
        {
            return 0;
        }

        // Use LINQ's .Count() method with a lambda to count students.
        // The lambda checks if the final mark is not NaN and is less than 50.
        return students.Count(s => {
            try
            {
                // Ensure final mark is valid before comparison
                return !double.IsNaN(s.GetFinalMark()) && s.GetFinalMark() < 50;
            }
            catch (CalculationException)
            {
                // If a student's mark cannot be calculated, they don't count towards being below 50.
                return false;
            }
        });
    }
}
