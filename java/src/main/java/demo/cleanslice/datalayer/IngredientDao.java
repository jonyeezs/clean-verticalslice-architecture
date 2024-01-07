package demo.cleanslice.datalayer;

import org.springframework.stereotype.Repository;

import java.sql.SQLException;

import com.j256.ormlite.dao.BaseDaoImpl;
import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.support.ConnectionSource;

import demo.cleanslice.datalayer.entities.Ingredient;

public interface IngredientDao extends Dao<Ingredient, Long> {

}

@Repository
class IngredientDaoImpl extends BaseDaoImpl<Ingredient, Long> implements IngredientDao {
    public IngredientDaoImpl(ConnectionSource connectionSource) throws SQLException {
        super(connectionSource, Ingredient.class);
    }
}
