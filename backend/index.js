
const express = require('express');
const path = require('path');
const bodyParser = require('body-parser');
const cors = require('cors');
const passport = require('passport');
const mongoose = require('mongoose');
const config = require('./config/database');


mongoose.connect(config.database);

mongoose.connection.on('connected', () => {
    console.log('connected');
});
mongoose.connection.on('error', (err) => {
    console.log('err');
});

const app = express();
const users = require('./routes/users');

const port = process.env.PORT || 8080;


app.use(cors());
//static folders


//body parser middleware
app.use(bodyParser.json());

// passport middleware
app.use(passport.initialize());
app.use(passport.session());
require('./config/passport')(passport);
app.use('/users', users);

app.get("/", (req, res) => {
    res.send("fnfffn");
});
app.get("*", (req, res) => {
    res.redirect('/');
});


app.listen(port, () => {
    console.log("running on port 8080");
});
