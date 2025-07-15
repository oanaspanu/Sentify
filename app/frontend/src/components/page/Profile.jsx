import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { api } from "../../service/api";
import Menu from "./Menu";
import '../style/Auth.css';

const Profile = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [userData, setUserData] = useState(null);
  const [editMode, setEditMode] = useState(false);
  const [formData, setFormData] = useState({ username: "", email: "", password: "" });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState("");

  useEffect(() => {
    if (!user || !user.id) {
      setError("No user is logged in.");
      setLoading(false);
      return;
    }

    const fetchUser = async () => {
      try {
        const res = await api.get(`/users/${user.id}`);
        setUserData(res.data);
        setFormData({ username: res.data.username, email: res.data.email, password: "" });
      } catch (err) {
        setError("Failed to fetch user data.");
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, [user]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleUpdate = async () => {
    setError("");
    setSuccess("");
    if (!formData.username || !formData.email || !formData.password) {
      setError("All fields are required.");
      return;
    }

    try {
      const res = await api.patch(`/users/${user.id}`, formData);
      setUserData((prev) => ({
        ...prev,
        username: res.data.username,
        email: res.data.email,
      }));
      setSuccess("Profile updated successfully.");
      setEditMode(false);
      setFormData((prev) => ({ ...prev, password: "" }));
    } catch (err) {
      setError("Failed to update profile.");
    }
  };

  const handleDelete = async () => {
    if (!window.confirm("Are you sure you want to delete your account? This action cannot be undone.")) {
      return;
    }

    try {
      await api.delete(`/users/${user.id}`);
      setSuccess("Account deleted. Redirecting...");
      logout && logout();
      navigate("/welcome");
    } catch (err) {
      setError("Failed to delete account.");
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p className="error-message">{error}</p>;

  return (
    <>
      <Menu />
      <div className="profile-container">
        <h2 className="auth-title">User Profile</h2>

        {success && <p className="success-message">{success}</p>}

        {editMode ? (
          <div className="auth-form">
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleInputChange}
              placeholder="Username"
              className="auth-input"
              style={{ marginRight: "10px" }}
            />
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleInputChange}
              placeholder="Email"
              className="auth-input"
              style={{ marginRight: "10px" }}
            />
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleInputChange}
              placeholder="Current or New Password"
              className="auth-input"
              style={{ marginRight: "10px" }}
            />
            <br />
            <button onClick={handleUpdate} className="btn-secondary">Save Changes</button>
            <br />
            <button onClick={() => setEditMode(false)} className="btn-secondary">Cancel</button>
          </div>
        ) : (
          <div className="auth-form">
            <p><strong>Username:</strong> {userData.username}</p>
            <p><strong>Email:</strong> {userData.email}</p>
            <button onClick={() => setEditMode(true)} className="btn-secondary">Change Profile Data</button>
            <hr className="divider" />
            <br />
            <button onClick={handleDelete} className="btn-primary">
              Delete Account
            </button>
          </div>
        )}

      </div>
    </>
  );
};

export default Profile;
