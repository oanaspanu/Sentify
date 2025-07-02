import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import "../style/Menu.css"; 

function Menu() {
    const { logout } = useAuth();
    const navigate = useNavigate();
    const [isDropdownOpen, setDropdownOpen] = useState(false);

    const handleLogout = () => {
        logout();
        navigate("/");
      };
    
      const toggleDropdown = () => {
        setDropdownOpen(!isDropdownOpen);
      };
    

    return (
        < nav className="menu" >
            <h1 className="menu-title">
                <img src="/icon.svg" alt="Sentify Logo" className="logo" />
                Sentify
            </h1>
            <div className="menu-links">
                <div className="hamburger-icon" onClick={toggleDropdown}>
                    <div></div>
                    <div></div>
                    <div></div>
                </div>

                <div className="menu-item">
                    {isDropdownOpen && (
                        <div className="dropdown-content">
                            <Link to="/dashboard" className="menu-link">Dashboard</Link>
                            <Link to="/text" className="menu-link">Analyse Text</Link>
                            <Link to="/youtube" className="menu-link">Analyse Comments</Link>
                            <Link to="/history" className="menu-link">Past analysis</Link>
                            <Link to="/profile" className="menu-link">Settings</Link>
                        </div>
                    )}
                </div>

                <button onClick={handleLogout} className="btn-primary">Logout</button>
            </div>
        </nav >
    );
}

export default Menu;
