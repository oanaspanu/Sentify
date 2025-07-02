import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import "../style/Auth.css";  

const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(username, password);
      navigate("/dashboard");
    } catch (err) {
      alert(err.message);
    }
  };

  const handleRegister = () => {
    navigate("/register");
  };

  return (
    <div className="auth-container">
      <div className="auth-form">
        <h1 className="auth-title">
          Welcome to Sentify
          <img src="/icon.svg" alt="logo" className="logo" />
        </h1>
        <form onSubmit={handleSubmit} className="form">
          <input
            className="auth-input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            placeholder="Username"
            required
          />
          <input
            className="auth-input"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Password"
            required
          />
          <button type="submit" className="btn-primary">
            Login
          </button>
        </form>
        <div className="subheading">
          Don’t have an account?{" "}
          <button onClick={handleRegister} className="btn-secondary">
            Register
          </button>
        </div>
      </div>
    </div>
  );
};

export default Login;
