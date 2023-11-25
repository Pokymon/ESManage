import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function CreateItem() {
  const [id, setId] = useState("");
  const [itemName, setItemName] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [categoryName, setCategoryName] = useState("");
  const [brandId, setBrandId] = useState("");
  const [uom, setUom] = useState("");
  const [taxType, setTaxType] = useState(0);
  const [taxRate, setTaxRate] = useState(0);
  const [minimumRetailPrice, setMinimumRetailPrice] = useState(0);
  const [balanceQty, setBalanceQty] = useState(0);
  const [avgCostPrice, setAvgCostPrice] = useState(0);
  const [retailPrice, setRetailPrice] = useState(0);
  const [costPrice, setCostPrice] = useState(0);
  const [createdBy, setCreatedBy] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);

  const navigate = useNavigate();

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  const handleCategoryChange = (e) => {
    const id = e.target.value;
    setCategoryId(id);
    // Find the category name using the selected id
    const selectedCategory = categories.find((c) => c.id === id);
    // Update categoryName in the state
    setCategoryName(selectedCategory ? selectedCategory.categoryName : "");
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const axiosConfig = {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        };
        const [categoriesRes, brandsRes] = await Promise.all([
          axios.get("https://localhost:7240/api/itemdepartment", axiosConfig),
          axios.get("https://localhost:7240/api/brand", axiosConfig),
        ]);
        setCategories(categoriesRes.data);
        setBrands(brandsRes.data);
      } catch (error) {
        console.error("Error fetching data:", error);
        setError("Failed to fetch data");
      }
    };

    fetchData();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      let axiosConfig = {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
        method: "POST",
        url: "https://localhost:7240/api/item",
        data: {
          id, // Assuming 'id' is part of your data model. Remove if not needed.
          itemName,
          categoryId,
          categoryName,
          brandId,
          uom,
          taxType,
          taxRate,
          minimumRetailPrice,
          balanceQty,
          avgCostPrice,
          retailPrice,
          costPrice,
          createdBy, // Assuming 'createdBy' is part of your data model. Remove if not needed.
        },
      };
      let response = await axios(axiosConfig);
      console.log(response.data);

      // Resetting all the fields to their initial states after successful submission
      setId("");
      setItemName("");
      setCategoryId("");
      setCategoryName("");
      setBrandId("");
      setUom("");
      setTaxType(0);
      setTaxRate(0);
      setMinimumRetailPrice(0);
      setBalanceQty(0);
      setAvgCostPrice(0);
      setRetailPrice(0);
      setCostPrice(0);
      setCreatedBy(""); // Reset this as well if 'createdBy' is part of your data model

      navigate("/item"); // Redirect to the item list page (or any other appropriate page)
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Create a new Item</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="row">
              <div className="mb-3 col-6">
                <label className="form-label">Item Name</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Enter the item name"
                  value={itemName}
                  onChange={(e) => setItemName(e.target.value)}
                />
              </div>

              {/* Category Dropdown */}
              <div className="mb-3 col-6">
                <label className="form-label">Category</label>
                <select
                  className="form-select"
                  value={categoryId}
                  onChange={handleCategoryChange} // Use the new handler here
                >
                  <option value="">Select a category</option>
                  {categories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.categoryName}
                    </option>
                  ))}
                </select>
              </div>

              {/* Brand Dropdown */}
              <div className="mb-3 col-6">
                <label className="form-label">Brand</label>
                <select
                  className="form-select"
                  value={brandId}
                  onChange={(e) => setBrandId(e.target.value)}
                >
                  <option value="">Select a brand</option>
                  {brands.map((brand) => (
                    <option key={brand.id} value={brand.id}>
                      {brand.name}
                    </option>
                  ))}
                </select>
              </div>

              {/* UOM Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Unit Of Measure</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Enter the unit of measure"
                  value={uom}
                  onChange={(e) => setUom(e.target.value)}
                />
              </div>

              {/* Tax Type Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Tax Type</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the tax type"
                  value={taxType}
                  onChange={(e) => setTaxType(e.target.value)}
                />
              </div>

              {/* Tax Rate Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Tax Rate</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the tax rate"
                  value={taxRate}
                  onChange={(e) => setTaxRate(e.target.value)}
                />
              </div>
              {/* Minimum Retail Price Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Minimum Retail Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the minimum retail price"
                  value={minimumRetailPrice}
                  onChange={(e) => setMinimumRetailPrice(e.target.value)}
                />
              </div>

              {/* Balance Quantity Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Balance Quantity</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the balance quantity"
                  value={balanceQty}
                  onChange={(e) => setBalanceQty(e.target.value)}
                />
              </div>

              {/* Average Cost Price Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Average Cost Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the average cost price"
                  value={avgCostPrice}
                  onChange={(e) => setAvgCostPrice(e.target.value)}
                />
              </div>

              {/* Retail Price Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Retail Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the retail price"
                  value={retailPrice}
                  onChange={(e) => setRetailPrice(e.target.value)}
                />
              </div>

              {/* Cost Price Field */}
              <div className="mb-3 col-6">
                <label className="form-label">Cost Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the cost price"
                  value={costPrice}
                  onChange={(e) => setCostPrice(e.target.value)}
                />
              </div>
            </div>
            <div className="mb-3">
              <input type="submit" value="Save" className="btn btn-primary" />
            </div>
            <div className="mb-3">
              <Link to="/item" className="btn btn-primary">
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

export default CreateItem;
