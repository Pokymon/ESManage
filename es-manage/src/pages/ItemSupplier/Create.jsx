import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function CreateItemSupplier() {
  const [id, setId] = useState("");
  const [itemId, setItemId] = useState("");
  const [supplierId, setSupplierId] = useState("");
  const [createdBy, setCreatedBy] = useState(""); // Assuming createdBy is needed
  const [items, setItems] = useState([]);
  const [suppliers, setSuppliers] = useState([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  useEffect(() => {
    const fetchItemsAndSuppliers = async () => {
      const axiosConfig = {
        headers: {
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
      };
      try {
        const [itemsResponse, suppliersResponse] = await Promise.all([
          axios.get("https://localhost:7240/api/item", axiosConfig),
          axios.get("https://localhost:7240/api/supplier", axiosConfig),
        ]);
        setItems(itemsResponse.data);
        setSuppliers(suppliersResponse.data);
      } catch (error) {
        setError("Failed to fetch items and suppliers");
      }
    };

    fetchItemsAndSuppliers();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const response = await axios.post(
        "https://localhost:7240/api/itemsupplier",
        { id, itemId, supplierId, createdBy },
        {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        }
      );
      console.log(response.data);
      setId("");
      setItemId("");
      setSupplierId("");
      navigate("/item-supplier");
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Create a new Item Supplier</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            {/* Item Dropdown */}
            <div className="mb-3">
              <label className="form-label">Item</label>
              <select
                className="form-select"
                value={itemId}
                onChange={(e) => setItemId(e.target.value)}
              >
                <option value="">Select an item</option>
                {items.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.itemName}
                  </option>
                ))}
              </select>
            </div>

            {/* Supplier Dropdown */}
            <div className="mb-3">
              <label className="form-label">Supplier</label>
              <select
                className="form-select"
                value={supplierId}
                onChange={(e) => setSupplierId(e.target.value)}
              >
                <option value="">Select a supplier</option>
                {suppliers.map((supplier) => (
                  <option key={supplier.id} value={supplier.id}>
                    {supplier.supplierName}
                  </option>
                ))}
              </select>
            </div>
            <div className="mb-3">
              <input type="submit" value="Save" className="btn btn-primary" />
            </div>
            <div className="mb-3">
              <Link to="/item-supplier" className="btn btn-primary">
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

export default CreateItemSupplier;
