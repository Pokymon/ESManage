import React, { useState, useEffect } from "react";
import axios from "axios";
import { useParams, useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function EditItemSupplier() {
  const { id: urlId } = useParams();
  const navigate = useNavigate();

  const [id, setId] = useState(urlId);
  const [itemId, setItemId] = useState("");
  const [supplierId, setSupplierId] = useState("");
  const [createdBy, setCreatedBy] = useState(""); // Assuming createdBy is part of your data model
  const [items, setItems] = useState([]);
  const [suppliers, setSuppliers] = useState([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      const axiosConfig = {
        headers: {
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
      };
      try {
        const [itemSupplierResponse, itemsResponse, suppliersResponse] =
          await Promise.all([
            axios.get(
              `https://localhost:7240/api/itemsupplier/${urlId}`,
              axiosConfig
            ),
            axios.get("https://localhost:7240/api/item", axiosConfig),
            axios.get("https://localhost:7240/api/supplier", axiosConfig),
          ]);

        const itemSupplierData = itemSupplierResponse.data;
        setId(itemSupplierData.id);
        setItemId(itemSupplierData.itemId);
        setSupplierId(itemSupplierData.supplierId);
        setItems(itemsResponse.data);
        setSuppliers(suppliersResponse.data);
      } catch (error) {
        console.error(error);
        setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [urlId]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      await axios.put(
        `https://localhost:7240/api/itemsupplier/${id}`,
        { id, itemId, supplierId, createdBy },
        {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        }
      );
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
      <h2 className="page-title">Edit Item Supplier</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">ID</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the ID"
                value={id}
                onChange={(e) => setId(e.target.value)}
                disabled
              />
            </div>
            {/* Dropdown for Item */}
            <div className="mb-3">
              <label className="form-label">Item</label>
              <select
                className="form-select"
                value={itemId}
                onChange={(e) => setItemId(e.target.value)}
                disabled
              >
                <option value="">Select an item</option>
                {items.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.itemName}
                  </option>
                ))}
              </select>
            </div>

            {/* Dropdown for Supplier */}
            <div className="mb-3">
              <label className="form-label">Supplier</label>
              <select
                className="form-select"
                value={supplierId}
                onChange={(e) => setSupplierId(e.target.value)}
                disabled
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

export default EditItemSupplier;
