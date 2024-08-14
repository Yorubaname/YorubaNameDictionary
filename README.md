# Yoruba Name Dictionary

Welcome to the Yoruba Name Dictionary! This repository contains two runnable projects: a Website and an API:
- The Website (https://www.yorubaname.com/) interacts with the API to deliver its functionality 
- The API is also used by [the dashboard](https://dashboard.yorubaname.com) in [another repo](https://github.com/Yorubaname/yorubaname-dashboard) to deliver its functionality

## Table of Contents
- [Projects](#projects)
- [Prerequisites](#prerequisites)
- [Setup and Running Locally](#setup-and-running-locally)
- [Contributing](#contributing)
- [Contact Information](#contact-information)

## Projects

### 1. Website
The Website project is the end-user-facing frontend of the application that communicates with the API to deliver the service to the users.

### 2. API
The API project handles the backend logic and data management, providing endpoints for the Website to consume. It uses a MongoDB database for data persistence.

## Prerequisites

To run this project locally, you will need to have installed:

- [Visual Studio 2019 or later](https://visualstudio.microsoft.com/vs/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Setup and Running Locally

To run the Website and API locally, follow these steps:

1. **Clone the repository:**

2. **Open the project in Visual Studio:**
    - Open Visual Studio.
    - Click on `Open a project or solution`.
    - Navigate to the cloned repository folder and select the `YorubaNameDictionary.sln` file.

3. **Set Docker Compose as the Startup project:**
    - In the Solution Explorer, locate the `docker-compose` project.
    - Right-click on the `docker-compose` project and select `Set as Startup Project`.

4. **Run the Docker Compose project:**
    - Click on the `Start` button (or press `F5`) to build and run the application.
    - This setup uses Docker to containerize the Website, the API and the database (MongoDB), allowing you to run and debug the application locally without manual installation of dependencies. 

5. **Access the Application:**
    - Once the Docker containers are up and running, you can access the Website in your browser. The website should launch automatically as soon as all the containers are ready.
    - If the Website does not launch automatically, it will be running at `http://localhost:{port}` (the actual port will be displayed in the output window of Visual Studio: "Container Tools").
    - The API will also be running locally and accessible via a different URL. You can see its documentation and test the endpoints at `http://localhost:51515/swagger`. You can login as any user with email and password: "Password@135".

## Contributing

Contributions are welcome! Please fork the repository and create a pull request for any feature or bug fix. Ensure your code adheres to the existing style.

## Contact Information
If you have any questions about the project or need more information/support to contribute to the project, feel free to reach out to the:
- Main developer: hoadewuyi@gmail.com
- Organization email: project@yorubaname.com
