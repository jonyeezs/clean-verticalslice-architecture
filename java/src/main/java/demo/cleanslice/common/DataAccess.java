package demo.cleanslice.common;

import java.sql.SQLException;

public interface DataAccess<TAggregateRoot, TOut> {
    TAggregateRoot Retrieve();

    TOut Add(TAggregateRoot domain) throws SQLException;
}
