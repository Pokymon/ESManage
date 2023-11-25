import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

function Login() {
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setIsLoading(true);

    const axiosConfig = {
      method: "post",
      url: "https://localhost:7240/api/auth/login",
      headers: {
        "Content-Type": "application/json",
      },
      data: {
        userName: userName,
        password: password,
      },
    };

    try {
      const response = await axios(axiosConfig);
      console.log(response.data);

      // Destructure the necessary data from the response
      const { accessToken } = response.data.token;
      const { userName: responseUserName, displayName } = response.data.user;

      // Store the retrieved information in localStorage
      localStorage.setItem("token", accessToken);
      localStorage.setItem(
        "user",
        JSON.stringify({ userName: responseUserName, displayName })
      );

      // Navigate to the dashboard with state
      navigate("/dashboard", {
        state: { userName: responseUserName, displayName },
      });
    } catch (error) {
      console.error(error);
      if (error.response) {
        setError(error.response.data);
      } else if (error.request) {
        setError("No response received");
      } else {
        setError("Error: " + error.message);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="border-top-wide border-primary d-flex flex-column">
      <div className="page page-center">
        <div className="container-tight py-4">
          <div className="text-center mb-4">
            <a href="." className="navbar-brand navbar-brand-autodark">
              <img src="{{ asset('dist/img/logo.svg') }}" height="36" alt="" />
            </a>
          </div>
          <form
            className="card card-md"
            onSubmit={handleSubmit}
            autoComplete="off"
          >
            <div className="card-body">
              <h2 className="card-title text-center mb-4">
                Login to your account
              </h2>
              <div className="mb-3">
                <label className="form-label">Username</label>
                <input
                  className="form-control"
                  placeholder="Enter username"
                  value={userName}
                  onChange={(e) => setUserName(e.target.value)}
                  required
                  autoFocus
                />
              </div>
              <div className="mb-3">
                <label className="form-label">Password</label>
                <div className="input-group input-group-flat">
                  <input
                    type="password"
                    className="form-control"
                    placeholder="Enter password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                </div>
              </div>
              <div className="form-footer">
                <button type="submit" className="btn btn-primary w-100">
                  Sign in
                </button>
              </div>
            </div>
          </form>
          {error && (
            <div className="alert alert-danger" role="alert">
              Error: {error}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default Login;
