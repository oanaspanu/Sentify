import { Routes, Route, Navigate } from "react-router-dom"; 
import Login from "./components/page/Login";
import Register from "./components/page/Register";
import Dashboard from "./components/page/Dashboard";
import Profile from "./components/page/Profile";
import TextAnalysis from "./components/analysis/TextAnalysis";
import YouTubeAnalysis from "./components/analysis/YouTubeAnalysis";
import History from "./components/analysis/History";
import { useAuth } from "./contexts/AuthContext";

function App() {
  const { user } = useAuth();

  return (
    <Routes>
      {user ? (
        <>
          <Route path="/" element={<Navigate to="/dashboard" />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/text" element={<TextAnalysis />} />
          <Route path="/youtube" element={<YouTubeAnalysis />} />
          <Route path="/history" element={<History />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="*" element={<Navigate to="/dashboard" />} />
        </>
      ) : (
        <>
          <Route path="/" element={<Navigate to="/login" />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="*" element={<Navigate to="/" />} />
        </>
      )}
    </Routes>
  );
}

export default App;
