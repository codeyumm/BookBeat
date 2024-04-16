# 📚 Book Beat - An ASP.NET Project

## 🌟 Project Overview

Book Beat is an ASP.NET project developed over 8 weeks by Saloni Pawar and Priyam Desai. It's an online platform for book and music enthusiasts to manage digital collections, share reviews, and discover new favorites.

## 🛠️ Technologies Used

- ASP.NET
- C#
- Entity Framework
- HTML
- CSS
- JavaScript

## 🚀 Main Features

1. **Profile Management**: Users can create, view, update, and delete profiles, including stats such as the number of books and music in their lists.
2. **Browse Books and Music**: Utilizes open-source APIs to fetch data, enabling users to explore various genres and titles.
3. **Like Books and Music**: Users can like books and music, facilitating engagement and ranking content.
4. **Review System**: Users can write and manage reviews for books and music, sharing insights and recommendations.
5. **Track Favorites**: Allows users to bookmark and save their favorite books and music for easy access.

## 🗄️ Database Structure

The project utilizes a relational database to manage user profiles, reviews, and media lists.

## 🏃‍♂️ Running the Project

1. **Change Target Framework**:
   - Navigate to `Project > BookBeat Properties`.
   - Change the target framework to `4.7.1`.
   - Change it back to `4.7.2`.
   - or Change target framework to 4.8.

2. **Verify App_Data Folder**:
   - Ensure there is an `App_Data` folder in the project.
   - Right-click the solution and select `View in File Explorer` to confirm its presence.

3. **Update Database**:
   - Open `Tools > NuGet Package Manager > Package Manager Console`.
   - Execute the command: `Update-Database`.

4. **Check Database Creation**:
   - Go to `View > SQL Server Object Explorer`.
   - Navigate to `MSSQLLocalDb`.
   - Verify that the database has been created.

5. **Run API Commands via CURL**:
   - Use CURL commands to interact with the API and create new animals.

## 🛠️ Common Issues & Resolutions

- **Issue:** (update-database) Could not attach .mdf database
  - **Solution:** Make sure App_Data folder is created.

- **Issue:** (update-database) Error. 'Type' cannot be null
  - **Solution:** (issue appears in Visual Studio 2022) Tools > Nuget Package Manager > Manage Nuget Packages for Solution > Install Latest Entity Framework version (eg. 6.4.4), restart Visual Studio and try again.

- **Issue:** (update-database) System Exception: Exception has been thrown by the target of an invocation
  - **Possible Solution:** Project was cloned to a OneDrive or other restricted cloud-based storage. Clone the project repository to the actual drive on the machine.

- **Issue:** (running server) Could not find part to the path ../bin/roslyn/csc.exe
  - **Solution:** Change target framework to 4.7.1 and back to 4.7.2. or Change target framework to 4.8.

- **Issue:** (running server) Project Failed to build. System.Web.Http does not have reference to serialize...
  - **Solution:** Solution Explorer > References > Add Reference > System.Web.Extensions.
    

## 🚀 Future Scope

Potential enhancements include notifications for shares and community features such as discussion groups.

## 👨‍💻 Contributors 

- Priyam Desai
- Saloni Pawar
