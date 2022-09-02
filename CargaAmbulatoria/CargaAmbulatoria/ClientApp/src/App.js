import React, { Component } from 'react';
import {  Route, Redirect } from 'react-router';
import { Layout } from './components/Layout';
import GuestRoutes from './routes/GuestRoutes';
import AuthenticatedRoutes from './routes/AuthenticatedRoutes';
import 'antd/dist/antd.css';
import './custom.css';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Redirect exact from={"/"} to={"/login"} />
                <Route path="/app" component={AuthenticatedRoutes} />
                <Route path="/" component={GuestRoutes} />
            </Layout>
        );
    }
}
