import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../service/api";
import { useAuth } from "../../contexts/AuthContext";
import "../style/Dashboard.css";
import Menu from "./Menu";

const Dashboard = () => {
  const { user, logout } = useAuth();
  const [userData, setUserData] = useState(null);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (!user || !user.id) {
      setError("No user is logged in.");
      return;
    }

    const fetchUser = async () => {
      try {
        const res = await api.get(`/users/${user.id}`);
        setUserData(res.data);
      } catch (err) {
        console.error("Error fetching user data:", err);
        setError("Failed to fetch user data.");
        logout();
        navigate("/");
      }
    };

    fetchUser();
  }, [user, logout, navigate]);

  if (error) return <p className="error-message">{error}</p>;

  return (
    <div className="dashboard-container">
      <Menu />
      <div className="dashboard-content">
        <h1>Welcome, {userData?.username}, to Sentify!</h1>

        <p>A machine learning-powered platform built to make sentiment analysis fast, intuitive, and insightful.
            Whether you're diving into YouTube comments or analyzing custom text, 
            Sentify uses intelligent models to uncover the emotional tone —positive, neutral, or negative— behind the words. 
            No need to scroll through endless feedback anymore.
        </p>
        <br />
        <p>Perfect for creators, marketers, researchers, or anyone who values audience insight, 
          Sentify fits your workflow: just paste a comment or drop a video link, and let our ML engine handle the rest.
        </p>
        <br />
        <h2>Let Sentify help you focus on decisions, not data crunching!</h2>

    </div>
    </div >
  );
};

export default Dashboard;
