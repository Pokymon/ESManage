import React, { useState, useEffect } from "react";
import axios from "axios";
import { useParams, useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function EditItem() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [item, setItem] = useState({
    itemName: "",
    categoryId: "",
    categoryName: "",
    brandId: "",
    uom: "",
    taxType: 0,
    taxRate: 0,
    minimumRetailPrice: 0,
    balanceQty: 0,
    avgCostPrice: 0,
    retailPrice: 0,
    costPrice: 0,
  });
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const getToken = () => {
    return localStorage.getItem("token");
  };

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const axiosConfig = {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        };
        const [itemRes, categoriesRes, brandsRes] = await Promise.all([
          axios.get(`https://localhost:7240/api/item/${id}`, axiosConfig),
          axios.get("https://localhost:7240/api/itemdepartment", axiosConfig),
          axios.get("https://localhost:7240/api/brand", axiosConfig),
        ]);
        setItem(itemRes.data);
        setCategories(categoriesRes.data);
        setBrands(brandsRes.data);
      } catch (error) {
        console.error("Error fetching data:", error);
        setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setItem((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  };

  const handleCategoryChange = (e) => {
    const selectedCategoryId = e.target.value;
    const selectedCategory = categories.find(
      (c) => c.id === selectedCategoryId
    );
    setItem((prevItem) => ({
      ...prevItem,
      categoryId: selectedCategoryId,
      categoryName: selectedCategory ? selectedCategory.categoryName : "",
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const axiosConfig = {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
        method: "PUT",
        url: `https://localhost:7240/api/item/${id}`,
        data: item,
      };
      const response = await axios(axiosConfig);
      console.log(response.data);
      navigate("/item");
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Edit Item</h2>
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
                  value={item.itemName} // Modified line
                  onChange={handleInputChange}
                  name="itemName" // Added line
                />
              </div>
              {/* Category Dropdown */}
              <div className="mb-3 col-6">
                <label className="form-label">Category</label>
                <select
                  className="form-select"
                  value={item.categoryId}
                  onChange={handleCategoryChange}
                  name="categoryId"
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
                  value={item.brandId}
                  onChange={handleInputChange}
                  name="brandId"
                >
                  <option value="">Select a brand</option>
                  {brands.map((brand) => (
                    <option key={brand.id} value={brand.id}>
                      {brand.name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Unit Of Measure</label>
                <input
                  type="text"
                  className="form-control"
                  placeholder="Enter the unit of measure"
                  value={item.uom}
                  onChange={handleInputChange}
                  name="uom"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Tax Type</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the tax type"
                  value={item.taxType}
                  onChange={handleInputChange}
                  name="taxType"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Tax Rate</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the tax rate"
                  value={item.taxRate}
                  onChange={handleInputChange}
                  name="taxRate"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Minimum Retail Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the minimum retail price"
                  value={item.minimumRetailPrice}
                  onChange={handleInputChange}
                  name="minimumRetailPrice"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Balance Quantity</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the balance quantity"
                  value={item.balanceQty}
                  onChange={handleInputChange}
                  name="balanceQty"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Average Cost Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the average cost price"
                  value={item.avgCostPrice}
                  onChange={handleInputChange}
                  name="avgCostPrice"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Retail Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the retail price"
                  value={item.retailPrice}
                  onChange={handleInputChange}
                  name="retailPrice"
                />
              </div>
              <div className="mb-3 col-6">
                <label className="form-label">Cost Price</label>
                <input
                  type="number"
                  className="form-control"
                  placeholder="Enter the cost price"
                  value={item.costPrice}
                  onChange={handleInputChange}
                  name="costPrice"
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

export default EditItem;
