# Project Summary

This system allows users to:

* Analyze **free-form text** or **YouTube video comments**
* View **sentiment classification results** (Positive, Neutral, Negative)
* Visualize **sentiment distribution** via charts
* Track **history of analyses**
* Register/login with **JWT-secured authentication**

---

## Architecture Overview

* **Frontend:** React SPA (React Router, Axios, Context API)
* **Backend:** ASP.NET Core Web API (JWT, Entity Framework Core, SQLite)
* **Machine Learning Service:** Python FastAPI microservice with a Logistic Regression model
* **Data Source:** YouTube Data API v3 & labeled YouTube comments dataset from Kaggle
* **ML Workflow:** TF-IDF Vectorization + Scikit-learn classification models

---

## Key Features

### Sentiment Analysis

* Classifies inputs into: Positive, Neutral, or Negative
* Uses a pre-trained Logistic Regression model (best performer among compared models)
* Real-time predictions powered by FastAPI

### YouTube Comment Analysis

* Fetches up to 100 top-level comments using YouTube Data API
* Aggregates and displays emotion distribution

### User Authentication

* Secure login/registration using **JWT**
* Passwords hashed with **BCrypt**
* User-specific analysis history

### Data Visualization

* Pie charts for comment sentiment trends
* Real-time feedback and error handling

---

## Installation and Startup Instructions

### Prerequisites

Create a `.env` file in your project root folder with the following environment variables:

```
YOUTUBE_API_KEY=your_youtube_api_key_here
DATABASE_URL=Data Source=app.db
JWT_SECRET_KEY=your_jwt_secret_key_here
```

Make sure to replace the placeholder values with your actual keys and secrets.

---

### Installing Dependencies and Starting Services

This project uses VS Code tasks to automate installing dependencies and starting all services.

* **Install all dependencies** by running the VS Code task:
  `Install All Services`

* **Start all services** by running the VS Code task:
  `Start All Services`

You can find the tasks configured in `.vscode/tasks.json`.

