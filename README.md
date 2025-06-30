# Sentiment Analysis Web App – Bachelor Thesis Project

A full-stack machine learning application that performs sentiment analysis on YouTube comments and custom text input using natural language processing (NLP) techniques. Developed as part of the **Bachelor Thesis** at the **Bucharest University of Economic Studies**.

---

## Project Summary

This system allows users to:
- Analyze **free-form text** or **YouTube video comments**
- View **sentiment classification results** (Positive, Neutral, Negative)
- Visualize **sentiment distribution** via charts
- Track **history of analyses**
- Register/login with **JWT-secured authentication**

---

## Architecture Overview

- **Frontend:** React SPA (React Router, Axios, Context API)
- **Backend:** ASP.NET Core Web API (JWT, Entity Framework Core, SQLite)
- **Machine Learning Service:** Python FastAPI microservice with a Logistic Regression model
- **Data Source:** YouTube Data API v3 & labeled YouTube comments dataset from Kaggle
- **ML Workflow:** TF-IDF Vectorization + Scikit-learn classification models

---

## Key Features

### Sentiment Analysis
- Classifies inputs into: Positive, Neutral, or Negative
- Uses a pre-trained Logistic Regression model (best performer among compared models)
- Real-time predictions powered by FastAPI

### YouTube Comment Analysis
- Fetches up to 100 top-level comments using YouTube Data API
- Aggregates and displays emotion distribution

### User Authentication
- Secure login/registration using **JWT**
- Passwords hashed with **BCrypt**
- User-specific analysis history

### Data Visualization
- Pie charts for comment sentiment trends
- Real-time feedback and error handling

---

## Technologies Used

| Layer        | Technology                   |
|-------------|------------------------------|
| Frontend    | React, React Router, Axios, Recharts |
| Backend     | ASP.NET Core, EF Core, SQLite, BCrypt.Net |
| ML Service  | Python, FastAPI, scikit-learn, TF-IDF |
| APIs        | YouTube Data API v3          |
| Tools       | Google Colab, VS Code, Postman, Kaggle |
