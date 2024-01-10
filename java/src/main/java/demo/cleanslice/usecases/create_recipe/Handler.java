package demo.cleanslice.usecases.create_recipe;

import java.sql.SQLException;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Service;

@Service
public class Handler {

    private DataAccess dataAccess;

    public Handler(@Qualifier("CreateRecipe") DataAccess dataAccess) {
        this.dataAccess = dataAccess;
    }

    public Response handle(Request request) throws SQLException, RecipeExistsException {
        var domain = this.dataAccess.Retrieve();

        var recipe = new Recipe(request.getTitle(), new Ingredient[0]);

        domain.addRecipe(recipe);

        var updatedDomain = this.dataAccess.Add(domain);

        return new Response(updatedDomain.getFirst());
    }
}
