const express = require('express');
const app = express();
const cors = require('cors');
const Pool = require('pg').Pool;

app.use(cors());
app.use(express.json()); // allows access to req.body

const pool = new Pool({
  user: 'es',
  password: '1234',
  host: 'localhost',
  port: 5432,
  database: 'esmanage',
});

app.get('/', (req, res) => {
  pool.query('SELECT * FROM your_table', (err, results) => {
    if (err) throw err;
    res.status(200).json(results.rows);
  });
});

app.listen(3001, () => {
  console.log('Server is running on port 3001');
});
