import React, { useState } from 'react';
import { api } from '../../service/api';
import Menu from '../page/Menu';
import '../style/Analysis.css';

const YouTubeAnalysis = () => {
  const [url, setUrl] = useState('');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleAnalyze = async (e) => {
    e.preventDefault();

    if (!url.trim()) {
      setError('Please enter a YouTube URL');
      return;
    }

    if (!isValidYouTubeUrl(url)) {
      setError('Invalid YouTube URL');
      return;
    }

    setLoading(true);
    setError('');
    setResult(null);

    try {
      const token = localStorage.getItem('token');
      const response = await api.post(
        '/sentiment/youtube/analyze',
        url.trim(),
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      setResult(response.data.distribution);
    } catch (err) {
      console.error(err);
      setError(
        err.response?.data?.message ||
          'Failed to analyze video. Please check the URL or try again later.'
      );
    } finally {
      setLoading(false);
    }
  };

  const isValidYouTubeUrl = (url) => {
    const youtubeRegex =
      /^(https?:\/\/)?(www\.youtube\.com|youtu\.be)\/(watch\?v=|embed\/|v\/|e\/|.+\/videoseries)\S+$/;
    return youtubeRegex.test(url);
  };

  return (
    <>
    <Menu />
    <div className="analysis-container">
      <h2 className="title">YouTube Sentiment Analysis</h2>
      <form onSubmit={handleAnalyze} className="analysis-form">
        <input
          type="text"
          value={url}
          onChange={(e) => setUrl(e.target.value)}
          placeholder="YouTube Video URL"
          className="analysis-input"
        />
        <br />
        <button type="submit" className="btn-primary">
          {loading ? 'Analyzing...' : 'Analyze'}
        </button>
      </form>

      {error && <p className="error-message">{error}</p>}

      {result && (
        <ul className="result">
          <h2>Analysis Result: </h2>
          {Object.entries(result).map(([label, percentage]) => (
            <li key={label}>
              <h2>{percentage}% {label}</h2>
            </li>
          ))}
        </ul>
      )}
    </div>
    </>
  );
};

export default YouTubeAnalysis;
