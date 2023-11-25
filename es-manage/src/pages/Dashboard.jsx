import React, { useState, useEffect } from "react";
import { useLocation, Link } from "react-router-dom"; // Import Link from react-router-dom
import AppLayout from "../layouts/AppLayout";

function Dashboard() {
  const [displayName, setDisplayName] = useState("Guest");

  const location = useLocation();
  useEffect(() => {
    // Attempt to retrieve user information from location state first
    const userFromState = location.state?.user || null;

    // Attempt to retrieve user information from localStorage as a fallback
    const userFromStorageItem = localStorage.getItem("user");
    let userFromStorage = null;
    try {
      if (userFromStorageItem && userFromStorageItem !== "undefined") {
        userFromStorage = JSON.parse(userFromStorageItem);
      }
    } catch (e) {
      console.error(e);
    }

    // Use the user information from state if available, otherwise from storage
    const user = userFromState || userFromStorage;

    // Set the display name if a user is found
    if (user && user.displayName) {
      setDisplayName(user.displayName);
    }
  }, [location]);

  return (
    <AppLayout>
      <h1>Welcome, {displayName}!</h1>
    </AppLayout>
  );
}

export default Dashboard;
