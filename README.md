# ProgressSoftDotNet


# Project Setup and Usage Guide

## Prerequisites
- Make sure you have Node.js version 20.9.0 or higher installed.
- Angular CLI version 17.0.10 or higher.
- npm (Node Package Manager) version 10.1.0 or higher.
- .NET Core SDK version (6.0).

> **Note**: You can download Node.js from [the official website](https://nodejs.org/), and npm will be installed automatically with Node.js. You can download .NET Core from [the official website](https://dotnet.microsoft.com/download).

## Steps to Set Up and Run the Application

1. **Clone the repository**:
   Clone the repository from GitHub using the following command:
   ```bash
   git clone https://github.com/EyasArqam/ProgressSoftDotNet.git
   ```
   Then, navigate to the project directory:
   ```bash
   cd ProgressSoftDotNet
   ```

2. **Install the required packages for the frontend**:
   navigate to the project 'Frontend' go to the path:
   ```bash
   cd ProgressSoftDotNet/BusinessCardClientApp
   ```
   To install all the necessary packages and libraries, run:
   ```bash
   npm install
   ```

3. **Run the frontend application**:
   Once the installation is complete, you can start the frontend application by running:
   ```bash
   ng s --o
   ```
   The application will be running on `http://localhost:4200/` by default. You can open a web browser and visit this address to see the application in action.

   > **Note**: If you do not want to install Angular CLI globally, you can use the following command instead:
   ```bash
   npx ng s --o
   ```

4. **Run the backend application**:
   To run the backend, navigate back to the root project directory:
   ```bash
   cd ..
   ```
   Next, navigate to the backend project directory `BusinessCardAPI`:
   ```bash
   cd ProgressSoftDotNet/BusinessCardAPI
   ```

   Restore the required packages:
   ```bash
   dotnet restore
   ```

   Finally, run the application:
   ```bash
   dotnet run
   ```

   The backend API will typically be available at `https://localhost:7281`.

   ## About the Projects in BusinessCardAPI
- **BusinessCardAPI**: This project handles the core business logic related to managing business card information, including creating, updating, and deleting cards, as well as providing APIs for the frontend to interact with the data.
- **BusinessCardAPI.Tests**: This project contains unit tests for the BusinessCardAPI project. It ensures that the application logic works as expected and that any changes to the code do not break existing functionality.

## Additional Notes
- Make sure to regularly update packages using `npm update` to ensure compatibility with the latest versions.
- If you encounter any issues during the installation or running of the application, ensure that the versions of Node.js and Angular CLI are as specified.


### 5. Configure the Database Connection
-restore the database from a dump file,
you can download it here 
https://drive.google.com/file/d/139y3zsAhyavAsWNLBwfPePe9L29kFpwX/view

- Once the restore process is complete, the next step is Configuration the ConnectionString.


## Configuration (appsettings.json)
The `appsettings.json` file contains essential configuration settings for the application. Make sure to review and adjust the settings as necessary for your environment.

ensure match your database server, database name, user ID, and password.


----------

# Usage

-There are two pages `Create Business Card` and `View All Business Cards`

## Page 1: Create Business Card
-The "Create Business Card" page is designed for creating a new business card for users

![Create Business Card](https://github.com/user-attachments/assets/bc9c9bc9-e861-4095-b56b-1f72b7603b80)

-There are two ways to create a business card in the application:
- **Import**: You can import business card data from a file, which allows for bulk creation of cards based on pre-existing information.

-To import business cards using XML or CSV, the file should follow these structure:

    1- ### XML Format
```xml
<?xml version="1.0" encoding="UTF-8"?>
<businessCards>
    <businessCard>
        <name>John Doe</name>
        <gender>Male</gender>
        <dateOfBirth>1990-01-01</dateOfBirth>
        <email>john.doe@example.com</email>
        <phone>123-456-7890</phone>
        <address>123 Main St, Springfield, USA</address>
    </businessCard>
    <businessCard>
        <name>Jane Smith</name>
        <gender>Female</gender>
        <dateOfBirth>1985-05-15</dateOfBirth>
        <email>jane.smith@example.com</email>
        <phone>098-765-4321</phone>
        <address>456 Elm St, Springfield, USA</address>
    </businessCard>
</businessCards>
```
2- ### CSV Format
```cvs
Name,Gender,DateOfBirth,Email,Phone,Address
John Doe,Male,1990-01-01,john.doe@example.com,123-456-7890,123 Main St
Jane Smith,Female,1985-05-15,jane.smith@example.com,098-765-4321,456 Elm St

```
  
- **Form**: You can also create business cards manually by filling out a form within the application. This method is useful for adding individual cards one at a time.



## Page 2: View All Business Cards

The "View All Business Cards" page displays a list of all created business cards. It includes features for filtering

![View All Business Cards](https://github.com/user-attachments/assets/13497c12-4fea-473a-b43c-6d919ad84ab2)


### Features:

1. **Business Cards Display**:
   - The page shows all the business cards in a grid format, each card including an image placeholder and basic information.

2. **Filter Options**:
   - There are several filter fields to help users search for specific cards:
     - **Name**: Filter by name.
     - **Email**: Filter by email address.
     - **Phone**: Filter by phone number.
     - **Choose a date**: Filter by date.
     - **Gender**: Filter by gender.

3. **Search Button**:
   - The "Search" button applies the selected filters to display matching results.

4. **Action Button**:
   - The "Action" button provides additional options when clicked, such as:
     - **Delete**: Remove a business card.
     - **Export to XML**: Export the selected business cards to an XML file.
     - **Export to CSV**: Export the selected business cards to a CSV file.




