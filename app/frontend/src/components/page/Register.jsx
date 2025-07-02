import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import "../style/Auth.css"; 

const Register = () => {
  const { register, login } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await register({ username, email, password });
      await login(username, password);
      navigate("/dashboard");
    } catch (err) {
      if (err.response && err.response.status === 400) {
        setError("Username or Email invalid or taken.");
      } else {
        setError("An error occurred. Please try again.");
      }
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-form">
        <h1 className="auth-title">Create Your Account</h1>

        {error && <div className="error-message">{error}</div>}

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
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email"
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
          <button type="submit" className="btn-primary mt-2">Register</button>
        </form>
      </div>
    </div>
  );
};

export default Register;
