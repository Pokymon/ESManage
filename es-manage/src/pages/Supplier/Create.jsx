import React, { useState } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function CreateSupplier() {
  const [id, setId] = useState("");
  const [supplierName, setSupplierName] = useState("");
  const [createdBy, setCreatedBy] = useState(""); // Corrected the casing of setCreatedBy
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      let response = await axios({
        method: "POST",
        url: "https://localhost:7240/api/supplier",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
        data: {
          id: id,
          supplierName: supplierName,
          createdBy: createdBy,
        },
      });
      console.log(response.data);
      setId("");
      setSupplierName("");
      setCreatedBy(""); // Reset createdBy
      navigate("/supplier");
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Create a new Supplier</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">Supplier Name</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the supplier name"
                value={supplierName}
                onChange={(e) => setSupplierName(e.target.value)}
              />
            </div>
            <div className="mb-3">
              <input type="submit" value="Save" className="btn btn-primary" />
            </div>
            <div className="mb-3">
              <Link to="/supplier" className="btn btn-primary">
                Cancel
              </Link>
            </div>
          </form>
          {error && (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          )}
        </div>
      </div>
    </AppLayout>
  );
}

export default CreateSupplier;
